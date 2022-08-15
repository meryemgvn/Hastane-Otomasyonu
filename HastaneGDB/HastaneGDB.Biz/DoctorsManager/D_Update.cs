using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HastaneGDB.Biz.DoctorsManager
{ 
  public class D_Update
    {
          HASTANEPROJEEntities gdb;
      
      public D_Update ()
	{
        gdb = new HASTANEPROJEEntities();
	}
      public byte Hasta_Guncelleme(string aciklama,bool? bakildimi,int kontrol_gunu,string tc ) {

       var hasta = (from i in gdb.HASTA where i.TCNO == tc select i).SingleOrDefault();
          hasta.ACIKLAMA = aciklama;
          hasta.BAKILDIMI= bakildimi;
          hasta.KONTROLGUNU = kontrol_gunu;
         var sonuc= gdb.SaveChanges();
         if (sonuc > 0)
         {
             return 1;
         }
         else
             return 0;
      }

      public byte Dr_Resim_Guncelle(byte [] resmim,string dr_ad) {
        var sonuc = (from i in gdb.DOKTORLAR where i.AD_SOYAD == dr_ad select i).SingleOrDefault();
        sonuc.FOTO = resmim;
        var resim_sonuc=gdb.SaveChanges();
        if (resim_sonuc > 0)
        {
            return 1;
        }
        else
            return 0;
      }
      public byte dakika_ekle(string tc,DateTime trh,TimeSpan ek_saat)
      {
          var sonuc = (from i in gdb.HASTA where i.TCNO == tc && i.TARIH==trh select i).SingleOrDefault();
          sonuc.SAAT = ek_saat;
          var eklenen = gdb.SaveChanges();
          if (eklenen > 0)
          {
              return 1;
          }
          else
              return 0;
      }

      public byte Hasta_Aktarma(string dr_ad, DateTime trh,string ak_dr_ad)
      {
          var sonuc = (from i in gdb.HASTA where i.DOKTORADI== dr_ad && i.TARIH == trh && i.RANDEVUMU==true select i).SingleOrDefault();
          sonuc.DOKTORADI = ak_dr_ad;
          var eklenen = gdb.SaveChanges();
          if (eklenen > 0)
          {
              return 1;
          }
          else
              return 0;

      }
      public byte Izinli(string dr_ad,string poli)
      {
          var sonuc = (from i in gdb.DOKTORLAR where i.AD_SOYAD== dr_ad && i.POLIKLINIK== poli select i).SingleOrDefault();
          sonuc.IZINLIMI = true;
          var guncel = gdb.SaveChanges();
          if (guncel > 0)
          {
              return 1;
          }
          else
              return 0;
        
      }



     
    }
    


}
