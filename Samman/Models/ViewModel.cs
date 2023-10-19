using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Samman.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Gender { get; set; }
        

    }

    public class RegisterBindingModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Gender { get; set; }

    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите имя пользователя")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите пароль")]
        public string Password { get; set; }
    }


    public class DocFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[]? FileContentPDF { get; set; }
        public byte[]? FileContentPNG { get; set; } // Допустимо null
        public byte[]? FileContentJPG { get; set; } // Допустимо null
        public byte[]? FileContentDOC { get; set; } // Допустимо null
        public DateTime DateCreated { get; set; }
    }

    public class ArchiveItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DocFilename { get; set; }

        public string DocFileTruename { get; set; }

        public string Category { get; set; }

        // Навигационное свойство для связи с PdfFile
        public DocFile? DocFile { get; set; }
    }

    public class DocFileViewModel
    {
        public int Id { get; set; }
        public string DocFileName { get; set; }

        public IFormFile? PdfFile { get; set; }
        public IFormFile? DocFile { get; set; }
        public IFormFile? JpgFile { get; set; }
        public IFormFile? PngFile { get; set; }


        public List<string> Categores { get; set; }

        public DateTime DateCreate { get; set; }
    }
}
