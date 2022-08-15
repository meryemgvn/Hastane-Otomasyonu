using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.GeneralManager
{
    public class G_Update
    {
        HASTANEPROJEEntities gdb;
        public G_Update()
        {
            gdb = new HASTANEPROJEEntities();
        }
        public void Bas_Resim_Guncelle(byte[] resmim,string bas_ad)
        {
            var sonuc = (from i in gdb.DOKTORLAR where i.AD_SOYAD == bas_ad select i).SingleOrDefault();
            sonuc.FOTO = resmim;
            gdb.SaveChanges();
        }

        public byte UpdateIzin(int id,string bas_aciklama)
        {
            var izin = (from i in gdb.IZIN where i.IZIN_ID == id select i).SingleOrDefault();
            izin.ONAYLANDIMI = false;
            izin.BAS_ACIKLAMA= bas_aciklama;
            var etkilenen= gdb.SaveChanges();
             if (etkilenen > 0)
            {
                return 1;
            }
            else
                return 0;
        }


        public byte UpdateDoktorIzinVer(string poli, string ad_soyad)
        {
            var izin = (from i in gdb.DOKTORLAR where i.AD_SOYAD == ad_soyad && i.POLIKLINIK == poli select i).SingleOrDefault();
            izin.IZINLIMI = true;
            var etkilenen = gdb.SaveChanges();
            if (etkilenen > 0)
            {
                return 1;
            }
            else
                return 0;
        }

        public byte UpdateIzinVer(int id)
        {
            var izin = (from i in gdb.IZIN where i.IZIN_ID == id select i).SingleOrDefault();
            izin.ONAYLANDIMI = true;
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
