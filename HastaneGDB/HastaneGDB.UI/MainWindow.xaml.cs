using HastaneGDB.Biz.InputManager;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HastaneGDB.Data;
using System.Windows.Media.Animation;

namespace HastaneGDB.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Query Query;
      
        public MainWindow()
        {
            Query = new Query();
            InitializeComponent();
            this.btn_giris.Click += btn_giris_Click;
            txt_tc.Focus();
        }

        void btn_giris_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_tc.Text))
            {
                Storyboard animasyon = (Storyboard)this.Resources["txt_ani"];
                animasyon.Begin();
            }

            else if (string.IsNullOrEmpty(txt_sifre.Password))
            {
                Storyboard animasyon = (Storyboard)this.Resources["txt_sifre_anim"];
                animasyon.Begin();
            }

            else
            {

                var kontrol = Query.Control(txt_tc.Text, txt_sifre.Password);
                switch ((int)kontrol)
                {
                    case 1:
                        this.Hide();
                        BASHEKIM bashekim = new BASHEKIM();
                        bashekim.ShowDialog(); 
                        break;
                    case 2:
                        this.Hide();
                        HEKIM hekim = new HEKIM();
                        hekim.ShowDialog();
                        break;
                    case 3:
                        this.Hide();
                        DANISMA danisma = new DANISMA();
                        danisma.ShowDialog();
                        break;
                    default:

                        Storyboard ani_uyari = (Storyboard)this.Resources["uyari_ani"];
                        ani_uyari.Begin();

                        break;
                }
            }
         
        }


      

    
        
    }
}
