using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.InformationManager
{
    
    public class H_Update
    {HASTANEPROJEEntities gdb;

        public H_Update ()
	{
        gdb = new HASTANEPROJEEntities();          
	}
        public byte G_Izin_Update(string ad,string poli)
        {
            var izin = (from i in gdb.DOKTORLAR where (i.AD_SOYAD != ad || i.POLIKLINIK != poli) && i.DR_ID!=0 select i).ToList();
            foreach (var item in izin)
            {
                item.IZINLIMI = false;
            }
            var etkilenen = gdb.SaveChanges();
            if (etkilenen > 0)
            {
                return 1;
            }
            else
                return 0;
        }
      
        public byte DanismaUpdate_DR(string ad,string poli)
        {
            var izin = (from i in gdb.DOKTORLAR where i.AD_SOYAD == ad && i.POLIKLINIK == poli select i).SingleOrDefault();
            izin.IZINLIMI = true;
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
