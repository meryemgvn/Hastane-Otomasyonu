using HastaneGDB.Biz.DoctorsManager;
using HastaneGDB.Data;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
    /// Interaction logic for HEKIM.xaml
    /// </summary>
    public partial class HEKIM : Window
    {
        D_Query d_query;
        D_Update d_update;
        D_Insert d_insert;
        public HEKIM()
        {
            d_update = new D_Update();
            d_query = new D_Query();
            d_insert = new D_Insert();
            InitializeComponent();
            this.Loaded += HEKIM_Loaded;
        }
        void HEKIM_Loaded(object sender, RoutedEventArgs e)
        {
            var sonuc = Application.Current.Windows.OfType<Window>().Where(s => s.Name == "WinGiris").FirstOrDefault();
            if (sonuc != null)
            {
                TextBox gelen = (TextBox)sonuc.FindName("txt_tc");
                dr_ad.Content = d_query.GetDoctorName(gelen.Text);
            }
            dr_poli.Content = d_query.GetDoctorPoli(dr_ad.Content.ToString());
            string tarih = DateTime.Now.ToShortDateString();
            DateTime ta = DateTime.Parse(tarih);
            var hastalar = d_query.GetHasta(ta, dr_ad.Content.ToString());
            dr_hastalar.ItemsSource = hastalar;


            MemoryStream stream = new MemoryStream();
            stream.Write(d_query.GetResim(dr_ad.Content.ToString()), 0, d_query.GetResim(dr_ad.Content.ToString()).Length);
            stream.Position = 0;
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            BitmapImage Imagee = new BitmapImage();
            Imagee.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            Imagee.StreamSource = ms;
            Imagee.EndInit();
            hekim_img.Source = Imagee;

            dr_izin_kontrol.ItemsSource = d_query.DR_IzinGecmisi(dr_ad.Content.ToString());
            cb_aktarilan_dr.ItemsSource = d_query.Dr_Aktarilan_Ad(dr_poli.Content.ToString(), dr_ad.Content.ToString());
            cb_aktarilan_dr.SelectedIndex = 0;
        }
        private void resim_degistir_Click(object sender, RoutedEventArgs e)
        {
            Storyboard resmi_degis = (Storyboard)this.Resources["resmi_degis"];
            resmi_degis.Begin();

            var dlg = new OpenFileDialog
            {
                Filter = "Resim Dosyaları|*.png;*.jpg;*.gif;*.bmp;*.png",
                Title = "Resmi Ekleyiniz...",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var bmp = new BitmapImage(new Uri(dlg.FileName, UriKind.Absolute));
                    hekim_img.Source = bmp;
                    PersonelResimSakla = dlg.FileName;

                    FileStream fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);

                    byte[] resmim = new byte[fs.Length];

                    fs.Read(resmim, 0, resmim.Length);

                    byte resimguncelleme_sonuc = d_update.Dr_Resim_Guncelle(resmim, dr_ad.Content.ToString());
                    if (resimguncelleme_sonuc > 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Yüklemeniz Başarıyla Gerçekleşmiştir.");
                    }
                }
                catch (Exception excp)
                {
                    MessageBox.Show("Hatalı İşlem Lütfen Tekrar Deneyiniz!", excp.Message);
                }
            }
        }
        private void dr_kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (dr_hastalar.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dr_hastalar.SelectedItems.Count; i++)
                {
                    
                    HASTA h = (HASTA)dr_hastalar.SelectedItem;
                    string aciklama = Convert.ToString(h.ACIKLAMA);
                    string tc = Convert.ToString(h.TCNO);
                    int kontrolgunu;
                    if (h.KONTROLGUNU == null)
                    {
                        kontrolgunu = 0;
                    }
                    else
                    {
                        kontrolgunu = (int)h.KONTROLGUNU.Value;
                    }


                    bool? bakildimi = false;
                    if (h.BAKILDIMI == true || h.KONTROLGUNU!= null || h.ACIKLAMA!= null)
                    {
                        bakildimi = true;
                    }
                    

                    byte sonuc = d_update.Hasta_Guncelleme(aciklama, bakildimi, kontrolgunu, tc);
                    if (sonuc > 0)
                    {
                        Storyboard kaydet = (Storyboard)this.Resources["kaydet_btn_ani"];
                        kaydet.Begin();

                        MessageBox.Show("Kayıt Başarılı!", "Bilgi!", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Kayıt Başarısız Tekrar Deneyiniz!", "HATA!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Henüz bir satır seçilmedi", "HATA!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public string PersonelResimSakla { get; set; }

        private void izin_iste_Click(object sender, RoutedEventArgs e)
        {
            var varmi = d_query.IzinliDoktorVarmi(dr_ad.Content.ToString(), dr_poli.Content.ToString());
            if (varmi)
            {
                if (izn_bas.SelectedDate.Value.DayOfWeek != DayOfWeek.Sunday && izn_bas.SelectedDate.Value.DayOfWeek != DayOfWeek.Saturday && izn_bit.SelectedDate.Value.DayOfWeek != DayOfWeek.Sunday && izn_bit.SelectedDate.Value.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (izn_bas.SelectedDate.Value >= DateTime.Now.Date && izn_bit.SelectedDate.Value >= DateTime.Now.Date)
                    {
                    string d_mail = d_query.GetDoktorMail(dr_ad.Content.ToString(), dr_poli.Content.ToString());
                    if (d_mail != "")
                    {
                        try
                        {
                            MailMessage eposta = new MailMessage();
                            eposta.From = new MailAddress(d_mail);
                            eposta.To.Add("eren.zede@gmail.com");
                            eposta.Subject = "İzin İsteği";
                            SmtpClient SMTP = new SmtpClient();
                            SMTP.Credentials = new System.Net.NetworkCredential(d_mail, "1111");
                            SMTP.Port = 587;
                            SMTP.Host = "smtp.gmail.com";
                            SMTP.EnableSsl = true;
                            object userState = eposta;
                            try
                            {
                                SMTP.SendAsync(eposta, (object)eposta);
                            }
                            catch (SmtpException mg)
                            {
                                MessageBox.Show(mg.Message, "Mail Gönderilemedi!");
                            }

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        MessageBox.Show("E-mailiniz Bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    try
                    {
                        byte istek_kayit = d_insert.Izin_Istegi(izn_bas.SelectedDate.Value, izn_bit.SelectedDate.Value, dr_ad.Content.ToString(), dr_poli.Content.ToString());

                        if (istek_kayit > 0)
                        {
                            MessageBox.Show("İzin İsteğiniz İşleme Alınmıştır.");
                        }
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show("İşlem Sırasında Hata Oluştu." + hata.Message);
                    }
                }
                    else
                        MessageBox.Show("Eski Bir Tarih Seçilemez!");
                }
                else
                    MessageBox.Show("Hafta Sonları Seçilemez!");
            }
            else
            {
                MessageBox.Show("Diğer Doktorlar İzinde Olduğundan, İzin Verilememiştir.");
            }
        }

        private void btn_aktar_Click(object sender, RoutedEventArgs e)
        {
            var varmi = d_query.IzinliDoktorVarmi(dr_ad.Content.ToString(), dr_poli.Content.ToString());
            if (varmi)
            {
                if (dt_aktar_tarih.SelectedDate.Value.DayOfWeek != DayOfWeek.Sunday && dt_aktar_tarih.SelectedDate.Value.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (dt_aktar_tarih.SelectedDate.Value >= DateTime.Now.Date) {
                    var randevu_varmi = d_query.RandevusuVarmi(dr_ad.Content.ToString(), dt_aktar_tarih.SelectedDate.Value);
                    if (randevu_varmi)
                    {
                        var A_randevu_varmi = d_query.RandevusuVarmi_ADoktor(cb_aktarilan_dr.SelectedValue.ToString(), dt_aktar_tarih.SelectedDate.Value);
                        if (A_randevu_varmi)
                        {
                            var randevulu_hastalar = d_query.Randevusu_Olan_Hastalar(dr_ad.Content.ToString(), dt_aktar_tarih.SelectedDate.Value);
                            foreach (var item in randevulu_hastalar)
                            {
                                var dr_randevusaat = (TimeSpan)item.SAAT;
                                var aktar_randevulu_hastalar = d_query.Aktar_Randevusu_Olan_Hastalar(cb_aktarilan_dr.SelectedValue.ToString(), dt_aktar_tarih.SelectedDate.Value);
                                foreach (var item2 in aktar_randevulu_hastalar)
                                {
                                    var ak_dr_randevusaat = (TimeSpan)item2.SAAT;
                                    if (dr_randevusaat.ToString() == ak_dr_randevusaat.ToString())
                                    {

                                        string tc = item.TCNO;
                                        TimeSpan ek = TimeSpan.FromMinutes(5);
                                        var sonuc2 = dr_randevusaat.Add(ek);
                                        byte eklendimi = d_update.dakika_ekle(tc, dt_aktar_tarih.SelectedDate.Value, sonuc2);
                                        if (eklendimi > 0)
                                        {
                                            System.Windows.Forms.MessageBox.Show("5 dakika eklenmiştir.");
                                        }
                                    }
                                }
                            }
                        }

                        var aktarildimi = d_update.Hasta_Aktarma(dr_ad.Content.ToString(), dt_aktar_tarih.SelectedDate.Value, cb_aktarilan_dr.SelectedValue.ToString());
                        if (aktarildimi > 0)
                        {
                            MessageBox.Show("Aktarma Yapılmıştır.", "Bilgilendirme!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    byte setgunluk_izin = d_insert.Gunluk_Izin(dt_aktar_tarih.SelectedDate.Value, aciklama_aktar.Text, dr_ad.Content.ToString(), dr_poli.Content.ToString(), cb_aktarilan_dr.SelectedValue.ToString());
                    if (setgunluk_izin > 0)
                    {
                        if (DateTime.Now.Date == dt_aktar_tarih.SelectedDate.Value)
                        {
                            byte update = d_update.Izinli(dr_ad.Content.ToString(), dr_poli.Content.ToString());
                        }

                    }

                    else
                    {
                        MessageBox.Show("İzin verirken hata oluştu", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }



                    MessageBox.Show("İzinlisiniz", "Bilgilendirme", MessageBoxButton.OK, MessageBoxImage.Information);


                    string d_mail = d_query.GetDoktorMail(dr_ad.Content.ToString(), dr_poli.Content.ToString());
                    if (d_mail != "")
                    {
                        try
                        {
                            MailMessage eposta = new MailMessage();
                            eposta.From = new MailAddress(d_mail);
                            eposta.To.Add("eren.zede@gmail.com");
                            eposta.Subject = "İzin İsteği";
                            SmtpClient SMTP = new SmtpClient();
                            SMTP.Credentials = new System.Net.NetworkCredential(d_mail, "1111");
                            SMTP.Port = 587;
                            SMTP.Host = "smtp.gmail.com";
                            SMTP.EnableSsl = true;
                            object userState = eposta;
                            try
                            {
                                SMTP.SendAsync(eposta, (object)eposta);
                            }
                            catch (SmtpException mg)
                            {
                                MessageBox.Show(mg.Message, "Mail Gönderilemedi!");
                            }

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        MessageBox.Show("E-mailiniz Bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                    else
                        MessageBox.Show("Eski Bir Tarih Seçilemez!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Hafta Sonu Seçilemez!", "Hata!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Diğer Doktorlar İzinde Olduğundan, İzin Verilememiştir.");
            }
        }


    }
}
