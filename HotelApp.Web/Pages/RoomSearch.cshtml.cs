using HotelAppLibrary.Data;
using HotelAppLibrary.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Pages
{
    public class RoomSearchModel : PageModel
    {
      private readonly IDatabaseData _db;

      [DataType(DataType.Date)]
      [Required]
      [BindProperty(SupportsGet = true)]
      public DateTime StartDate { get; set; } = DateTime.Now;

      [DataType( DataType.Date )]
      [Required]
      [BindProperty( SupportsGet = true )]
      public DateTime EndDate { get; set; } = DateTime.Now.AddDays( 1 );


      [BindProperty( SupportsGet = true )]
      public bool SearchEnabled { get; set; } = false;

      public List<RoomTypeModel> AvailableRoomTypes { get; set; }

      // This will give back an Instance of SqlData (Dependency Injection)
      public RoomSearchModel(IDatabaseData db) 
      {
         _db = db;
      }


      public void OnGet()
      {
         if (SearchEnabled == true)
         {
            /*
            This will give me back a list of RoomTypeModel
            The return of RoomTypeModel goes into AvailableRoomTypes.
            We have just done data access in ASP.Net6 Razor page application,
            but we didn't actually touch the database. We didn't configure Dapper, none of that stuff.
            That's all done at the Class Library, which is done once.
            And no our quote on quote data access is just asked for it.
            We don't have to worry about what's happening here or even
            which IDatabaseData instance we are getting. All we care about is
            when we say GetAvailableRoomTypes pass in the start and end dates, we are going to get
            back available room types, that's it

            */
            AvailableRoomTypes = _db.GetAvailableRoomTypes(StartDate, EndDate);
         }

      }

      public IActionResult OnPost()
      {

         return RedirectToPage(new 
         { 
            SearchEnabled = true, 
            StartDate = StartDate.ToString("yyyy-MM-dd"),   // Changed to HTML expected format for the date.
            EndDate = EndDate.ToString( "yyyy-MM-dd" )      // Changed to HTML expected format for the date.
         } );
      }
    }
}
