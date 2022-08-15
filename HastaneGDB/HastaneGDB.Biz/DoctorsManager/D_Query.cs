using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.DoctorsManager
{
    public class D_Query
    {
            HASTANEPROJEEntities gdb;

            public D_Query()
            {
                gdb = new HASTANEPROJEEntities();
            }

            public string GetDoctorName(string doctor_tc)
            {
                return (from i in gdb.GIRIS where i.TCNO == doctor_tc select i.ADSOYAD).SingleOrDefault();
            }

            public string GetDoctorPoli(string doctor_adi)
            {
                return (from i in gdb.DOKTORLAR where i.AD_SOYAD == doctor_adi select i.POLIKLINIK).SingleOrDefault();
            }

            public List<HASTA> GetHasta(DateTime bugun, string doctor_adi)
            {
                return (from i in gdb.HASTA where i.DOKTORADI == doctor_adi && i.TARIH == bugun select i).ToList();
            }

            public byte [] GetResim(string dr_ad)
            {
                return (from i in gdb.DOKTORLAR where i.AD_SOYAD == dr_ad select i.FOTO).SingleOrDefault();
            }

            public bool IzinliDoktorVarmi(string ad,string poli)
            {
                return (from i in gdb.DOKTORLAR where i.AD_SOYAD!=ad && i.POLIKLINIK==poli && i.IZINLIMI==false select i).Any();
            }

            public string GetDoktorMail(string ad, string poli)
            {
                return (from i in gdb.DOKTORLAR where i.AD_SOYAD == ad && i.POLIKLINIK == poli select i.E_MAIL).SingleOrDefault();
            }

        public List<IZIN> DR_IzinGecmisi(string ad){
            return (from i in gdb.IZIN where i.AD_SOYAD==ad select i).ToList();
        }

        public List<string> Dr_Aktarilan_Ad(string poli,string dr_ad)
        {
            return (from i in gdb.DOKTORLAR where i.AD_SOYAD != dr_ad && i.POLIKLINIK == poli && i.IZINLIMI==false select i.AD_SOYAD).ToList();
        }

        public bool RandevusuVarmi(string dr_ad, DateTime s_tarih)
        {
            return (from i in gdb.HASTA where i.DOKTORADI == dr_ad && i.TARIH == s_tarih && i.RANDEVUMU == true select i).Any();
        }
        public bool RandevusuVarmi_ADoktor(string dr_ad, DateTime s_tarih)
        {
            return (from i in gdb.HASTA where i.DOKTORADI == dr_ad && i.TARIH == s_tarih && i.RANDEVUMU == true select i).Any();
        }
        public List<HASTA> Randevusu_Olan_Hastalar(string dr_ad,DateTime s_tarih)
        {
            return(from i in gdb.HASTA where i.DOKTORADI==dr_ad && i.TARIH==s_tarih && i.RANDEVUMU==true select i).ToList();
        }
        public List<HASTA> Aktar_Randevusu_Olan_Hastalar(string ak_dr_ad, DateTime s_tarih)
        {
            return (from i in gdb.HASTA where i.DOKTORADI == ak_dr_ad && i.TARIH == s_tarih && i.RANDEVUMU == true select i).ToList();
        }
           

    }
}
