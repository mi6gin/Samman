
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Samman.Models;
using SammanWebSite.Models;

namespace Samman.DataBase
{
    public class DocFileDbContext : DbContext
    {
        public DbSet<DocFile> DocFile { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database/Base/pdf.db");
        }

        public DocFileDbContext()
        {
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string docFolderPath = Path.Combine(currentDirectory, "wwwroot/content/doc");
            string pdfFolderPath = docFolderPath;


            if (Database.EnsureCreated())
            {
                if (Directory.Exists(pdfFolderPath))
                {
                    var fileFormats = new Dictionary<string, DocFile>();
                    var random = new Random();

                    foreach (var filePath in Directory.GetFiles(pdfFolderPath, "*.*")
                        .Where(file => file.ToLower().EndsWith(".pdf") || file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".doc")))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        var startDate = new DateTime(2000, 1, 1);
                        var endDate = new DateTime(2022, 12, 31);
                        var randomDate = startDate.AddDays(random.Next((endDate - startDate).Days));

                        if (fileFormats.ContainsKey(fileName))
                        {
                            var docFile = fileFormats[fileName];

                            if (filePath.EndsWith(".pdf"))
                            {
                                docFile.FileContentPDF = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".jpg"))
                            {
                                docFile.FileContentPNG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".png"))
                            {
                                docFile.FileContentJPG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".doc"))
                            {
                                docFile.FileContentDOC = File.ReadAllBytes(filePath);
                            }
                        }
                        else
                        {
                            var docFile = new DocFile
                            {
                                FileName = fileName,
                                DateCreated = randomDate
                            };

                            if (filePath.EndsWith(".pdf"))
                            {
                                docFile.FileContentPDF = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".jpg"))
                            {
                                docFile.FileContentPNG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".png"))
                            {
                                docFile.FileContentJPG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".doc"))
                            {
                                docFile.FileContentDOC = File.ReadAllBytes(filePath);
                            }

                            fileFormats[fileName] = docFile;
                        }
                    }

                    DocFile.AddRange(fileFormats.Values);
                    SaveChanges();
                }
            }
        }
    }
}
