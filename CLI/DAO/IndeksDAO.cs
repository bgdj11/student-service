﻿using CLI.Model;
using CLI.Storage;

using System.Collections.Generic;
using System.Linq;

namespace CLI.DAO
{
     public class IndeksDAO
    {
        private readonly List<Indeks> _indeksi;
        private readonly Storage<Indeks> _storage;

        public IndeksDAO()
        {
            _storage = new Storage<Indeks>("indeksi.csv");
            _indeksi = _storage.Load();
        }

        public int GenerateID()
        {
            if (_indeksi.Count == 0) return 0;
            return _indeksi[^1].idIndeksa + 1;
        }

        public int GetLastID()
        {
            return _indeksi[^1].idIndeksa;
        }

        public Indeks AddIndeks(Indeks indeks)
        {
            indeks.idIndeksa = GenerateID();
            _indeksi.Add(indeks);
            _storage.Save(_indeksi);
            return indeks;
        }

        public Indeks? UpdateIndeks(Indeks indeks)
        {
            Indeks? oldIndeks = GetIndeksById(indeks.idIndeksa);
            if (oldIndeks == null) return null;

            oldIndeks.oznakaSmera = indeks.oznakaSmera;
            oldIndeks.brojUpisa = indeks.brojUpisa;
            oldIndeks.godinaUpisa = indeks.godinaUpisa;

            _storage.Save(_indeksi);
            return oldIndeks;
        }

        public Indeks? RemoveIndeks(int id)
        {
            Indeks? indeks = GetIndeksById(id);
            if (indeks == null) return null;

            _indeksi.Remove(indeks);
            _storage.Save(_indeksi);
            return indeks;
        }

        private Indeks? GetIndeksById(int id)
        {
            return _indeksi.Find(i => i.idIndeksa == id);
        }

        public List<Indeks> GetAllIndeks()
        {
            return _indeksi;
        }

    }
}
