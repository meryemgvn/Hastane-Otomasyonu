using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.InformationManager
{
    public class H_Insert
    {
        HASTANEPROJEEntities gdb;
        public H_Insert()
        {
            gdb = new HASTANEPROJEEntities();
            
        }


        public byte Hasta_Kaydi(string tc, string adsoyad, string poliklinik, string doktor_ad, string adres, string tel, bool sskmi, bool randevumu, DateTime tarih, TimeSpan saat)
        {
            gdb.HASTA.Add(new HASTA { TCNO = tc, ADI_SOYADI = adsoyad, POLIKLINIK = poliklinik, DOKTORADI = doktor_ad, ADRES = adres, TEL = tel, SSKMI = sskmi, RANDEVUMU = randevumu, TARIH = tarih, SAAT = saat,BAKILDIMI=false });

            var etkilenen = gdb.SaveChanges();

            if (etkilenen > 0)
            {
                return 1;
            }
            else
                return 0;
        }


    }
}
