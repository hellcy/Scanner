using Scanner.Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Scanner.Controllers.api
{
    public class FileUpldController : ApiController
    {
        // GET api/<controller>
        public FolderFiles Get()
        {
            string MainFolder = ConfigurationManager.AppSettings["MainFolder"].ToString();
            DirectoryInfo dir = new DirectoryInfo(@MainFolder);
            IList<string> folderAndFiles = new List<string>();
            IList<string> names = new List<string>();
            int idx = 0;
            folderAndFiles.Add("<ul>");
            AddDirectories(ref folderAndFiles, ref names, ref idx, dir);
            folderAndFiles.Add("</ul>");

            FolderFiles ff = new FolderFiles();
            ff.folderAndFiles = folderAndFiles;
            ff.names = names;

            return ff;
        }


        public FolderFiles Post(string obj)
        {
            string MainFolder = ConfigurationManager.AppSettings["MainFolder"].ToString();
            DirectoryInfo dir = new DirectoryInfo(@MainFolder);
            IList<string> folderAndFiles = new List<string>();
            IList<string> names = new List<string>();
            int idx = 0;
            folderAndFiles.Add("<ul>");
            AddDirectories(ref folderAndFiles, ref names, ref idx, dir);
            folderAndFiles.Add("</ul>");

            FolderFiles ff = new FolderFiles();
            ff.folderAndFiles = folderAndFiles;
            ff.names = names;



            return ff;
        }


        private void AddDirectories(ref IList<string> folderAndFiles, ref IList<string> names, ref int idx, DirectoryInfo dir)
        {
            string path = "";
            if (dir.GetDirectories().Count() > 0)
            {
                folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openDir(this.id)' data='" + dir.FullName.Replace(@"C:\", @"\").Replace(@"\", "/") + "'>" + dir.Name + "</a><ul>");
                names.Add(dir.Name);
                idx++;
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    AddDirectories(ref folderAndFiles, ref names, ref idx, d);
                }


                if (dir.GetFiles().Count() > 0)
                {
                    foreach (FileInfo f in dir.GetFiles())
                    {
                        // folderAndFiles.Add("<li><a href='#' onclick='$(\"#byClick\").val(\"1\");window.open(\""+@f.FullName+"\")'>" + f.Name + "</a></li>");
                        // folderAndFiles.Add("<li><a href='file:/" + @f.FullName + "'>" + f.Name + "</a></li>");
                        path = f.FullName.Replace(@"C:\", @"\GramEngineering\").Replace(@"\", "/");
                        folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openFile(this.id)' data='" + path + "'>" + f.Name + "</a></li>");
                        names.Add(f.Name);
                        idx++;
                    }
                }

                folderAndFiles.Add("</ul></li>");
            }
            else
            {

                folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "'  onclick='openDir(this.id)' data='" + dir.FullName.Replace(@"C:\", @"\").Replace(@"\", "/") + "'>" + dir.Name + "</a><ul>");
                names.Add(dir.Name);
                idx++;
                if (dir.GetFiles().Count() > 0)
                {
                    foreach (FileInfo f in dir.GetFiles())
                    {
                        // folderAndFiles.Add("<li><a href='file:/"+@f.FullName+"'>" + f.Name + "</a></li>");

                        // folderAndFiles.Add("<li><a onclick='openFile(\""+ @f.FullName.Replace(@"\", "/") + "\")'>" + f.Name + "</a></li>");
                        path = f.FullName.Replace(@"C:\", @"\GramEngineering\").Replace(@"\", "/");
                        folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openFile(this)' data='" + path + "'>" + f.Name + "</a></li>");
                        names.Add(f.Name);
                        idx++;

                    }
                }
                folderAndFiles.Add("</ul></li>");
                var aa = folderAndFiles.Count();
            }
        }

    }
}