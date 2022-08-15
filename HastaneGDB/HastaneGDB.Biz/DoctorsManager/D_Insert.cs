using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.DoctorsManager
{
  public  class D_Insert
    {
      HASTANEPROJEEntities gdb;
      public D_Insert()
      {
          gdb = new HASTANEPROJEEntities();
      }
       public byte Izin_Istegi(DateTime bas_tarih,DateTime bit_tarih,string ad,string poli)
       {
           gdb.IZIN.Add(new IZIN { BASLANGICTARIH = bas_tarih, BITISTARIH = bit_tarih, AD_SOYAD = ad, POLIKLINIK = poli });
           var sonuc = gdb.SaveChanges();
           if (sonuc > 0)
           {
               return 1;
           }
           else
               return 0;
       }

       public byte Gunluk_Izin(DateTime tarih,string aciklama, string dr_ad, string poli,string ak_drad)
       {
           gdb.GUNLUKIZIN.Add(new GUNLUKIZIN { TARIH=tarih,ACIKLAMA=aciklama,AD_SOYAD=dr_ad,POLIKLINIK=poli,AKTAR_AD_SOYAD=ak_drad});
           var sonuc = gdb.SaveChanges();
           if (sonuc > 0)
           {
               return 1;
           }
           else
               return 0;
       }
    }
}
