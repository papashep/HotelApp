using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelAppLibrary.Models;
public class BookingFullModel
{
   public int Id { get; set; }

   public int RoomTypeId { get; set; }

   public int GuestId { get; set; }

   public DateTime StartDate { get; set; }

   public DateTime EndDate { get; set; }

   public Boolean CheckedIn { get; set; }

   public decimal TotalCost { get; set; }

   public string FirstName { get; set; }

   public string LastName { get; set; }

   public string RoomNumber { get; set; }

   public int RoomTypeID { get; set; }
   
   public string Title { get; set; }

   public string Description { get; set; }

   public decimal Price { get; set; }
}
