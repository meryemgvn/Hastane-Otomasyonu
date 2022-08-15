using HastaneGDB.Biz.GeneralManager;
using HastaneGDB.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
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
    /// Interaction logic for BASHEKIM.xaml
    /// </summary>
    public partial class BASHEKIM : Window
    {
        G_Query g_query;
        G_Update g_update;
        public BASHEKIM()
        {
            g_query = new G_Query();
            g_update = new G_Update();
            InitializeComponent();
            this.Loaded += BASHEKIM_Loaded;
        }

        void BASHEKIM_Loaded(object sender, RoutedEventArgs e)
        {
            var sonuc = Application.Current.Windows.OfType<Window>().Where(s => s.Name == "WinGiris").FirstOrDefault();
            if (sonuc != null)
            {
                TextBox b_gelen = (TextBox)sonuc.FindName("txt_tc");
                bashekim_ad.Content = g_query.GetBasHekimName(b_gelen.Text);
            }
            string tarih = DateTime.Now.ToShortDateString();
            DateTime ta = DateTime.Parse(tarih);
            dg_bas_gunlukizin.ItemsSource = g_query.GetGunlukizin(ta);

            dg_bas_izin.ItemsSource = g_query.GetIzin();

            MemoryStream stream = new MemoryStream();
            stream.Write(g_query.GetBasResim(bashekim_ad.Content.ToString()), 0, g_query.GetBasResim(bashekim_ad.Content.ToString()).Length);
            stream.Position = 0;
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            BitmapImage Imagee = new BitmapImage();
            Imagee.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            Imagee.StreamSource = ms;
            Imagee.EndInit();
            bas_image.Source = Imagee;

            cb_bas_hastatakip.ItemsSource = g_query.Bas_GetDoctor();
            cb_bas_hastatakip.SelectedIndex = 0;
        }
        private void bas_resim_degistir_Click_1(object sender, RoutedEventArgs e)
        {
            Storyboard resmi_degis = (Storyboard)this.Resources["bas_resimdegis_ani"];
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
                    bas_image.Source = bmp;
                    PersonelResimSakla = dlg.FileName;

                    FileStream fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);

                    byte[] resmim = new byte[fs.Length];

                    fs.Read(resmim, 0, resmim.Length);


                    g_update.Bas_Resim_Guncelle(resmim, bashekim_ad.Content.ToString());
                    System.Windows.Forms.MessageBox.Show("Yüklemeniz Başarıyla Gerçekleşmiştir.");

                }
                catch (Exception excp)
                {
                    MessageBox.Show("Hatalı İşlem Lütfen Bildiriniz !", excp.Message);
                }
            }
        }
        public string PersonelResimSakla { get; set; }

        private void btn_bashekim_yenile_Click(object sender, RoutedEventArgs e)
        {
            DateTime hasta_yenile = date_bas_hasta.SelectedDate.Value;

            dg_bas_hastatakip.ItemsSource = g_query.Bas_HastaTakip(hasta_yenile);
        }

        private void btn_bas_hastalistele_Click(object sender, RoutedEventArgs e)
        {
            string dr_ad = cb_bas_hastatakip.SelectedValue.ToString();
            DateTime ilk_tarih = dt_ilk_dr_hastatakip.SelectedDate.Value;
            DateTime son_tarih = dt_son_dr_hastatakip.SelectedDate.Value;
            dg_bas_drtakip.ItemsSource = g_query.Bas_DoktorTakip(dr_ad, ilk_tarih,son_tarih);
        }
        private void btn_izin_verme_Click(object sender, RoutedEventArgs e)
        {
            if (dg_bas_izin.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dg_bas_izin.SelectedItems.Count; i++)
                {
                    IZIN n = (IZIN)dg_bas_izin.SelectedItem;
                    string aciklama = txt_aciklama.Text;
                    int id = Convert.ToInt32(n.IZIN_ID);
                    string adsoyad = n.AD_SOYAD;
                    string poli = n.POLIKLINIK;
                    byte sonuc = g_update.UpdateIzin(id, aciklama);
                    if (sonuc > 0)
                    {
                        MessageBox.Show("Izin Onaylanmamıştır!", "BİLGİ!", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        string d_mail = g_query.B_GetDoktorMail(adsoyad, poli);
                        if (d_mail != "")
                        {
                            try
                            {

                                MailMessage eposta = new MailMessage();
                                eposta.From = new MailAddress(d_mail);
                                eposta.To.Add("erenzede@gmail.com");
                                eposta.Subject = "İzin İsteği Cevap";
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
                    {
                        MessageBox.Show("Tekrar Deneyiniz!", "HATA!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Henüz bir satır seçilmedi", "HATA!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_izin_ver_Click(object sender, RoutedEventArgs e)
        {
            if (dg_bas_izin.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dg_bas_izin.SelectedItems.Count; i++)
                {
                    IZIN n = (IZIN)dg_bas_izin.SelectedItem;
                    int id = Convert.ToInt32(n.IZIN_ID);
                    string adsoyad = n.AD_SOYAD;
                    string poli = n.POLIKLINIK;
                    var sonuc = g_update.UpdateIzinVer(id);

                    if (sonuc > 0)
                    { if (DateTime.Now.Date == n.BASLANGICTARIH)
                        {
                            var sonuc2 = g_update.UpdateDoktorIzinVer(poli, adsoyad);
                        }
                        MessageBox.Show("İzin Onaylanmıştır!", "BİLGİ!", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                       

                        string d_mail = g_query.B_GetDoktorMail(adsoyad, poli);
                        if (d_mail != "")
                        {
                            try
                            {
                                MailMessage eposta = new MailMessage();
                                eposta.From = new MailAddress(d_mail);
                                eposta.To.Add("erenzede@gmail.com");
                                eposta.Subject = "İzin İsteği Cevap";
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
                    {
                        MessageBox.Show("Tekrar Deneyiniz!", "HATA!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Henüz bir satır seçilmedi", "HATA!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btn_ResmiGoruntule_Click(object sender, RoutedEventArgs e)
        {
            if (dg_bas_izin.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dg_bas_izin.SelectedItems.Count; i++)
                {
                    IZIN b = (IZIN)dg_bas_izin.SelectedItem;
                    string adsoyad = b.AD_SOYAD;
                    MemoryStream streamm = new MemoryStream();
                    streamm.Write(g_query.GetIzinliResmi(adsoyad), 0, g_query.GetIzinliResmi(adsoyad).Length);
                    streamm.Position = 0;
                    System.Drawing.Image imgg = System.Drawing.Image.FromStream(streamm);
                    BitmapImage resim = new BitmapImage();
                    resim.BeginInit();
                    MemoryStream mss = new MemoryStream();
                    imgg.Save(mss, System.Drawing.Imaging.ImageFormat.Bmp);
                    mss.Seek(0, SeekOrigin.Begin);
                    resim.StreamSource = mss;
                    resim.EndInit();
                    img_izinli.Source = resim;
                    Storyboard izin_resim = (Storyboard)this.Resources["izin_resim_ani"];
                    izin_resim.Begin();
                }
            }
            else
            {
                MessageBox.Show("Henüz bir satır seçilmedi", "HATA!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_yazdir_Click(object sender, RoutedEventArgs e)
        {
            dg_bas_hastatakip.SelectAllCells();
            dg_bas_hastatakip.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dg_bas_hastatakip);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dg_bas_hastatakip.UnselectAllCells();
            SaveFileDialog save = new SaveFileDialog() { Filter = "Excel files|*.xls", DefaultExt = "xls" };
            if (save.ShowDialog() == true)
            {
                File.WriteAllText(save.FileName, "Doktor_Takip");
            }
           
             save.Title = "Excel Dosyasını Kaydediniz...";
             save.RestoreDirectory = true;
             save.CreatePrompt = true;
             save.InitialDirectory = @"c:\temp\";
             save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            System.IO.StreamWriter file = new System.IO.StreamWriter(@save.FileName);
            file.WriteLine(result.Replace(',', ' '));
            file.Close();
 
            MessageBox.Show(" Excel'e başarıyla aktarılmıştır");


        }

        private void btn_yazdirma_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog Yazdir = new PrintDialog();

            if (Yazdir.ShowDialog() == true)
            {
                Yazdir.PrintVisual((yazdir), "Data Çıktısı");

            }

            else
            {
                MessageBox.Show("Hiç seçim yapılmadı.");
            }
        }

       


    }
}

