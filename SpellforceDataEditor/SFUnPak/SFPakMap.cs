﻿/*
 * SFPakMap contains a set of SFPakFileSystem objects, each bound to a unique PAK archive,
 *      and methods for retrieving binary data of specified files from any PAK (or some of them) in that set
 * For convenience, SFPakMap can preload data from directory for later use to speed up loading times
 * */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFUnPak
{
    public class SFPakMap
    {
        Dictionary<string, SFPakFileSystem> pak_map= new Dictionary<string, SFPakFileSystem>();

        public SFPakMap()
        {

        }

        public int AddPak(string pak_fname)
        {
            if (!File.Exists(pak_fname))
                return -4;

            SFPakFileSystem fs = new SFPakFileSystem();
            int open_result = fs.Init(pak_fname);
            if (open_result != 0)
            {
                System.Diagnostics.Debug.WriteLine("ERROR PAK INIT " + pak_fname + " CODE" + open_result.ToString());
                return open_result;
            }

            pak_map.Add(Path.GetFileName(pak_fname), fs);
            return 0;
        }

        public SFPakFileSystem GetPak(string pak_name)
        {
            if (pak_map.ContainsKey(pak_name))
                return pak_map[pak_name];
            return null;
        }

        public int SaveData(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(pak_map.Count);
            foreach (KeyValuePair<string, SFPakFileSystem> kv in pak_map)
            {
                bw.Write(kv.Key);
                kv.Value.WriteToFile(bw);
            }
            bw.Close();
            fs.Close();
            return 0;
        }

        public int LoadData(string fname)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            }
            catch(Exception e)
            {
                return -2;
            }
            BinaryReader br = new BinaryReader(fs);
            pak_map.Clear();
            int num = br.ReadInt32();
            for(int i = 0; i < num; i++)
            {
                string key = br.ReadString();
                SFPakFileSystem value = new SFPakFileSystem();
                value.ReadFromFile(br);
                pak_map.Add(key, value);
            }
            br.Close();
            fs.Close();
            return 0;
        }

        public List<String> ListAllWithExtension(string path, string extname, string[] pak_filter)
        {
            List<String> names = new List<String>();
            if (pak_filter == null)
                pak_filter = pak_map.Keys.ToArray();
            foreach(string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[pak];
                names = names.Union(fs.ListAllWithExtension(path, extname)).ToList();
            }
            return names;
        }

        public void Clear()
        {
            foreach (SFPakFileSystem sys in pak_map.Values)
                sys.Dispose();
            pak_map.Clear();
        }
    }
}
