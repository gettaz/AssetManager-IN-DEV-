using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class Price
    {
        public string Ticker { get; set; }

        public DateTime Date { get; set; }

        public double Value { get; set; }
    }
}
