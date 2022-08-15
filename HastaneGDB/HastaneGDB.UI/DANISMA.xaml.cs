using HastaneGDB.Biz.InformationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HastaneGDB.UI
{
    /// <summary>
    /// Interaction logic for DANISMA.xaml
    /// </summary>
    public partial class DANISMA : Window
    {

        H_Insert H_Insert;
        H_Query H_Query;
        H_Update H_Update;
        public DANISMA()
        {

            H_Update = new H_Update();
            H_Insert = new H_Insert();
            H_Query = new H_Query();
            InitializeComponent();
            this.Loaded += DANISMA_Loaded;
        }

        void DANISMA_Loaded(object sender, RoutedEventArgs e)
        {
           
            var izin = H_Query.G_Izin_Bittimi(DateTime.Now.Date);
            foreach (var m in izin)
            {
                string ad = m.AD_SOYAD;
                string po = m.POLIKLINIK;
                byte izini_bulunmayan = H_Update.G_Izin_Update(ad, po);
            }

            var izin_kontrol = H_Query.G_Izinli_DrVarmi(DateTime.Now.Date);
            foreach (var k in izin_kontrol)
            {
                string adi = k.AD_SOYAD;
                string poliklinik = k.POLIKLINIK;
                byte varsa = H_Update.DanismaUpdate_DR(adi, poliklinik);             
            }


            var poli = H_Query.GetPoliklinik();
            cb_poliklinik.ItemsSource = poli;
            cb_poliklinik.SelectedIndex = 0;

            txt_tc.Focus();

        }
        private void sgk_Checked(object sender, RoutedEventArgs e)
        {

            Storyboard sgk = (Storyboard)this.Resources["sgk_ani"];
            sgk.Begin();
            Storyboard ess = (Storyboard)this.Resources["es_ani"];
            ess.Stop();
        }

        private void btn_cikis_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_yeni_Click(object sender, RoutedEventArgs e)
        {
            txt_tc.Clear();
            txt_adsoyad.Clear();
            txt_adres.Clear();
            txt_tel.Clear();
            randevu.IsChecked = false;
            rd_muayene.IsChecked = false;
            sgk.IsChecked = false;
            emekli.IsChecked = false;
            cb_doktoradi.SelectedIndex = 0;
            cb_poliklinik.SelectedIndex = 0;
            btn_kaydet.IsEnabled = true;
            dg_hastagecmis.Columns.Clear();
            dg_hastagecmis.ItemsSource = null;
  
            Storyboard sgkk = (Storyboard)this.Resources["sgk_ani"];
            sgkk.Stop();
            Storyboard ess = (Storyboard)this.Resources["es_ani"];
            ess.Stop();
        }

        private void btn_kaydet_Click_1(object sender, RoutedEventArgs e)
        {

            if (txt_tc.Text == "" || txt_adsoyad.Text == "" || txt_tel.Text == "" || txt_adres.Text == "")
            {

                Storyboard basarisiz = (Storyboard)this.Resources["basarisiz_like"];
                basarisiz.Begin();
                Storyboard basarisiz2 = (Storyboard)this.Resources["basarisiz_yazi"];
                basarisiz2.Begin();

            }
            else
            {
                var tarihim = DateTime.Now.ToShortDateString();

                TimeSpan saatim = DateTime.Now.TimeOfDay;

                var kayit_kontrol = H_Insert.Hasta_Kaydi(txt_tc.Text, txt_adsoyad.Text, cb_poliklinik.SelectedItem.ToString(), cb_doktoradi.SelectedItem.ToString(), txt_adres.Text, txt_tel.Text, sgk.IsChecked.Value, randevu.IsChecked.Value, DateTime.Parse(tarihim), saatim);

                if ((byte)kayit_kontrol == 1)
                {
                    Storyboard basarili = (Storyboard)this.Resources["basarili_like"];
                    basarili.Begin();
                    Storyboard basarili2 = (Storyboard)this.Resources["basarili_yazi"];
                    basarili2.Begin();
                }

                else
                {

                    Storyboard basarisiz = (Storyboard)this.Resources["basarisiz_like"];
                    basarisiz.Begin();
                    Storyboard basarisiz2 = (Storyboard)this.Resources["basarisiz_yazi"];
                    basarisiz2.Begin();
                }
            }
        }

        private void emekli_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard es = (Storyboard)this.Resources["es_ani"];
            es.Begin();
            Storyboard sgk = (Storyboard)this.Resources["sgk_ani"];
            sgk.Stop();
        }
        private void cb_poliklinik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string p = cb_poliklinik.SelectedItem.ToString();
            var doctorad = H_Query.GetDoctor(p);
            cb_doktoradi.ItemsSource = doctorad;
            cb_doktoradi.SelectedIndex = 0;
        }

        private void txt_tc_LostFocus_1(object sender, RoutedEventArgs e)
        {
            rd_muayene.IsChecked = true;
            var randevum = H_Query.IsRandevu(txt_tc.Text);
           
            foreach (var i in randevum)
            {
                txt_adsoyad.Text = i.ADI_SOYADI;
                txt_adres.Text = i.ADRES;
                txt_tel.Text = i.TEL;
                cb_poliklinik.SelectedItem = i.POLIKLINIK;
                cb_doktoradi.SelectedItem = i.DOKTORADI;
                if (i.RANDEVUMU.Value)
                {
                    randevu.IsChecked = true;
                    btn_kaydet.IsEnabled = false;
                }
                else
                {
                    rd_muayene.IsChecked = true;
                }

                if (i.SSKMI.Value)
                {
                    sgk.IsChecked = true;
                }
                else
                {
                    emekli.IsChecked = true;
                }
            }

            if (txt_tc.Text.Length < 11 || txt_tc.Text.Length > 11)
            {
                Storyboard balon = (Storyboard)this.Resources["balon_ani"];
                balon.Begin();
            }

            var kontrol = H_Query.KontrolGunu(txt_tc.Text);

            if (kontrol != null)
            {
                foreach (var k in kontrol)
                {
                    DateTime tarihi = Convert.ToDateTime(k.TARIH);
                    int kontrolgunu = (int)k.KONTROLGUNU;
                    DateTime bugun = DateTime.Now.Date;
                   
                    TimeSpan GunFarki = bugun - tarihi;
                    int sonuc = kontrolgunu - Convert.ToInt32(GunFarki.Days);
                    if (sonuc > 0)
                    {
                        MessageBox.Show(sonuc.ToString() + " Gün Sonra Geliniz!", "Kontrol Günü!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        dg_hastagecmis.ItemsSource = H_Query.Hasta_Gecmisin(txt_tc.Text);
                        dg_hastagecmis.RowBackground = Brushes.SkyBlue;
                        dg_hastagecmis.IsEnabled = false;
                    }
                }
            }
        }
        private void btn_h_gecmis_Click(object sender, RoutedEventArgs e)
        {

            dg_hastagecmis.ItemsSource = H_Query.Hasta_Gecmisin(txt_tc.Text);
        }
        private void btn_yenile_Click(object sender, RoutedEventArgs e)
        {

            string tarih = DateTime.Now.ToShortDateString();
            DateTime ta = DateTime.Parse(tarih);
            dg_hastatakip.ItemsSource = H_Query.HastaTakip(ta);
           
        }


       






    }
}
