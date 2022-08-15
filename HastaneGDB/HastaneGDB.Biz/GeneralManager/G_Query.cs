using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.GeneralManager
{
   public class G_Query
    {
       HASTANEPROJEEntities gdb;
       public G_Query()
       {
           gdb = new HASTANEPROJEEntities(); 
       }
       public string GetBasHekimName(string bashekim_tc)
       {
           return (from i in gdb.GIRIS where i.TCNO == bashekim_tc select i.ADSOYAD).SingleOrDefault();
       }

       public List<GUNLUKIZIN> GetGunlukizin(DateTime bugun)
       {
           return (from i in gdb.GUNLUKIZIN where i.TARIH == bugun select i).ToList();
       }

       public List<IZIN> GetIzin()
       {
           return (from i in gdb.IZIN where i.ONAYLANDIMI != false && i.ONAYLANDIMI != true select i).ToList();
       }

       public byte[] GetBasResim(string dr_ad)
       {
           return (from i in gdb.DOKTORLAR where i.AD_SOYAD == dr_ad select i.FOTO).SingleOrDefault();
       }

       public List<HASTA> Bas_HastaTakip(DateTime tarih)
       {
           return (from i in gdb.HASTA where i.TARIH == tarih select i).ToList();
      }

       public List<string> Bas_GetDoctor()
       {
           return (from i in gdb.DOKTORLAR where i.DR_ID!=0 select i.AD_SOYAD).Distinct().ToList();
       }


       public List<HASTA> Bas_GetDoctor(string dr_ad)
       {
           return (from i in gdb.HASTA where i.DOKTORADI == dr_ad select i).ToList();
       }

       public List<HASTA> Bas_DoktorTakip(string dr_ad,DateTime ilk_trh,DateTime son_trh)
       {

           return (from i in gdb.HASTA where i.TARIH >= ilk_trh && i.TARIH <= son_trh && i.DOKTORADI==dr_ad select i).ToList();
       }

       public byte[] GetIzinliResmi(string ad)
       {
           return (from i in gdb.DOKTORLAR where i.AD_SOYAD == ad select i.FOTO).SingleOrDefault();
       }
       public string B_GetDoktorMail(string ad, string poli)
       {
           return (from i in gdb.DOKTORLAR where i.AD_SOYAD == ad && i.POLIKLINIK == poli select i.E_MAIL).SingleOrDefault();
       }
    }
}
