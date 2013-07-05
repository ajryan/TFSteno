using System;

namespace TFSteno.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string TfsUrl { get; set; }
        public string TfsUsername { get; set; }
        public string TfsPassword { get; set; }
    }
}