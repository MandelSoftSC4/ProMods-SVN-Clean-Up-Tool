using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMD_usage_filter
{
    class Program
    {
        static void Main()
        {
            // Main definitions
            string homedir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string format_asset = ".pmd";
            string format_def   = ".sii";
            string format_def2  = ".sui";
            string filedest1 = "assets_unused.txt";
            string filedest2 = "assets_unmatched.txt";
            //string filelist_asset = (homedir + "\\filelist_asset.txt");
            //string filelist_def = (homedir + "\\filelist_def.txt");
            //string strCmdText;
            string line;
            string relpath;
            string assetpath;
            int n;
            int numel;

            // Our lists
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();
            List<string> assets_exist = new List<string>();
            List<string> paths_def = new List<string>();
            List<string> assets_used = new List<string>();
            List<string> assets_unused = new List<string>();

            // Our file writers
            System.IO.StreamWriter filewrite1 = new System.IO.StreamWriter(filedest1);
            System.IO.StreamWriter filewrite2 = new System.IO.StreamWriter(filedest2);

            // Mount all the bases you want to search through
            string pathfile = "basepath.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(pathfile);
            List<string> mount_path = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                mount_path.Add(line);
            }
            file.Close();
            // The first entry will be the one you want to clean up.
            string basepath = mount_path[0];


            for (int j = 0; j < mount_path.Count; j++)
            {
                Console.WriteLine("Gathering files and DEFs from: " + mount_path[j]);
                string[] dir1 = System.IO.Directory.GetFiles(mount_path[j], "*.PMD", System.IO.SearchOption.AllDirectories);
                string[] dir2 = System.IO.Directory.GetFiles(mount_path[j], "*.SII", System.IO.SearchOption.AllDirectories);
                string[] dir3 = System.IO.Directory.GetFiles(mount_path[j], "*.SUI", System.IO.SearchOption.AllDirectories);


                ////Getting our PMD files
                //strCmdText = ("/C dir /B /S \"" + mount_path[j] + "\\*" + format_asset + "\" > \"" + filelist_asset + "\"");
                //Console.WriteLine("Getting asset file list from: " + mount_path[j]);
                //System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                ////Getting our DEF files
                //strCmdText = ("/C dir /B /S \"" + mount_path[j] + "\\*" + format_def + "\" > \"" + filelist_def + "\"");
                //Console.WriteLine("Getting DEF file list from: " + mount_path[j]);
                //System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //// Let's check if we actually have a file list for the assets
                //if ((File.Exists(filelist_asset)) == false)
                //{
                //Console.WriteLine("No assets file list not found!");
                //Console.ReadLine();
                //return;
                //}
                //else
                //{
                //Console.WriteLine("Assets file list created!");
                //}
                //file = new System.IO.StreamReader(filelist_assets);

                for (int k = 0; k < dir1.Count(); k++)
                {
                    if (dir1[k].EndsWith(format_asset))
                    {
                        relpath = GetRelPath(dir1[k], mount_path[j]);
                        list1.Add(relpath);
                    }
                }
                //Console.WriteLine("Asset file list read!");
                //file.Close();

                // Let's check if we actually have a file list for the DEFs
                //if ((File.Exists(filelist_def)) == false)
                //{
                //    Console.WriteLine("No DEF file list not found!");
                //    Console.ReadLine();
                //    return;
                //}
                //else
                //{
                //    Console.WriteLine("DEF file list created!");
                //}

                //file = new System.IO.StreamReader(filelist_def);
                for (int k = 0; k < dir2.Count(); k++)
                {
                    if (dir2[k].EndsWith(format_def))
                    {
                        relpath = GetRelPath(dir2[k], mount_path[j]);
                        paths_def.Add(relpath);
                    }
                }
                for (int k = 0; k < dir3.Count(); k++)
                {
                    if (dir3[k].EndsWith(format_def2))
                    {
                        relpath = GetRelPath(dir3[k], mount_path[j]);
                        paths_def.Add(relpath);
                    }
                }
                //Console.WriteLine("DEF file list read!");
                //file.Close();

                Console.WriteLine("Gathering asset references from DEF files");
                numel = paths_def.Count();

                n = 0;
                for (int i = 0; i < numel; i++)
                {
                    string filename = (mount_path[j] + paths_def[i]);
                    file = new System.IO.StreamReader(filename);
                    //Console.WriteLine("Reading: " + filename);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains(format_asset))
                        {
                            if (line.StartsWith("#") == false)
                            {
                            line = NoWhiteSpace(line);
                            int dem1 = line.IndexOf("\"");
                            int dem2 = line.IndexOf("\"", dem1+1);
                            assetpath = (line.Substring(dem1 + 1, dem2 - dem1 - 1));
                            list2.Add(assetpath);
                            n++;
                            }
                        }
                    }
                    file.Close();
                }
                paths_def.Clear();
            }
            // We are now sorting the list and remove the double entries.
            Console.WriteLine("Sorting ...");
            list1.Sort();
            list2.Sort();

            // Remove double entries from the existing assets list
            n = 0;
            numel = list1.Count();
            for (int i = 0; i < numel; i++)
            {
                if (i > 0)
                {
                    if (list1[i] != list1[i - 1])
                    {
                        line = list1[i];
                        assets_exist.Add(line);
                        n++;
                    }

                }
                else
                {
                    line = list1[i];
                    assets_exist.Add(line);
                    n++;
                }

            }

            // Remove double entries from the used assets list
            n = 0;
            numel = list2.Count();
            for (int i = 0; i < numel; i++)
            {
                if (i > 0)
                {
                    if (list2[i] != list2[i - 1])
                    {
                        line = list2[i];
                        line = line.Replace('/', '\\');
                        assets_used.Add(line);
                        n++;
                    }

                }
                else
                {
                    line = list2[i];
                    line = line.Replace('/', '\\');
                    assets_used.Add(line);
                    n++;
                }

            }

            //Let the comparision begin!
            n = 0;
            int m;
            int f;
            bool nomatch;
            int n_nm = 0;
            int n_uu = 0;
            Console.WriteLine("Unused models found: " + n_uu + "; Matches not found: " + n_nm);
            for (int i = 0; i < assets_used.Count(); i++)
            {
                nomatch = false;
                m = n;
                f = 0;
                while (assets_used[i] != assets_exist[m])
                {
                    m++;
                    f++;
                    if (m >= assets_exist.Count())
                    {
                        n++;
                        break;
                    }

                    // Sanity check
                    if (f > 10)
                    {
                        string[] ordercheck = { assets_exist[m], assets_used[i] };
                        Array.Sort(ordercheck);
                        if (ordercheck[0] != assets_exist[m])
                        {
                            nomatch = true;
                            n_nm++;
                            filewrite2.WriteLine(assets_used[i]);
                            break;
                        }
                        else
                        { f = 0; }
                    }
                }
                int x_unused = m - n;
                if (nomatch == false)
                    { 
                    if (x_unused > 1)
                    {
                        for (int k = n; k < m; k++)
                        {
                            n_uu++;
                            assets_unused.Add(assets_exist[k]);
                            filewrite1.WriteLine(assets_exist[k]);
                        }
                    }
                    n = m + 1;
                }
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
                Console.WriteLine("Unused models found: " + n_uu + "; Matches not found: " + n_nm);
            }
            filewrite1.Close();
            filewrite2.Close();
            Console.WriteLine("Done!");

            //    // Hey, cleaning lady! We need to clean-up some files!
            //    try
            //    {
            //        System.IO.File.Delete(filedest1);
            //    }
            //    catch (System.IO.IOException e)
            //    {
            //        Console.WriteLine(e.Message);
            //        Console.ReadLine();
            //        return;
            //    }
            //}
        }
        // So, here we define some functions...
        public static string NoWhiteSpace(string line)
        {
            string linetrim = line.Trim();
            return linetrim;
        }
        public static string GetRelPath(string line, string basepath)
        {
            int den = basepath.Length;
            string result = (line.Substring(den, (line.Length) - den));
            return result;
        }
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}

