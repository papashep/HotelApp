using HotelAppLibrary.Databases;
using HotelAppLibrary.Models;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotelAppLibrary.Data;
public class SqliteData : IDatabaseData
{
   private readonly ISqliteDataAccess _db;

   private const string connectionStringName = "SqliteDB";
   public SqliteData( ISqliteDataAccess db )
   {
      _db = db;
   }

   string sql = "";
   public void BookGuest( string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId )
   {
      sql = @"SELECT 1 FROM Guests WHERE FirstName = @firstName AND LastName = @lastName;";
      
      int results = _db.LoadData<dynamic, dynamic>( sql, new { firstName, lastName }, connectionStringName ).Count();
      
      if ( results == 0 )
      {
         sql = @"INSERT INTO Guests (FirstName, LastName)
                     VALUES (@firstName, @lastName);";

         _db.SaveData( sql,new { firstName, lastName }, connectionStringName );
      }

      sql = @"SELECT Id, FirstName, LastName
               FROM Guests
               WHERE FirstName = @firstName AND LastName = @lastName LIMIT 1;";


      GuestModel guest = _db.LoadData<GuestModel, dynamic>( sql,
                                                           new { firstName, lastName },
                                                           connectionStringName).First();

      RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>( "SELECT * FROM RoomTypes WHERE Id = @Id",
                                                                    new { Id = roomTypeId },
                                                                    connectionStringName ).First();                                                                 
      TimeSpan timeStaying = endDate.Date.Subtract( startDate.Date );

      sql = @"SELECT r.* FROM Rooms r
	                INNER JOIN RoomTypes t ON t.Id = r.RoomTypeId
	                WHERE r.RoomTypeId = @roomTypeId AND r.Id NOT IN (
	                SELECT b.RoomId FROM Bookings b
	                WHERE (@startDate < b.StartDate AND @endDate > b.EndDate)
		                OR (b.StartDate <= @endDate AND @endDate < b.EndDate)
		                OR (b.StartDate <= @startDate AND @startDate < b.EndDate));";

      List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>( sql,
                                                                        new { startDate, endDate, roomTypeId },
                                                                        connectionStringName );

      sql = @"INSERT INTO Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
              VALUES(@roomId, @guestId, @startDate, @endDate, @totalCost);";

      _db.SaveData( sql,
                   new
                   {
                      roomId = availableRooms.First().Id,
                      guestId = guest.Id,
                      startDate = startDate,
                      endDate = endDate,
                      totalCost = timeStaying.Days * roomType.Price
                   },
                   connectionStringName );                
   }

   public void CheckInGuest( int bookingId )
   {
      sql = @"UPDATE Bookings
              SET CheckedIn = 1 
              WHERE Id = @Id;";

      _db.SaveData( sql,
                  new { Id = bookingId },
                  connectionStringName );
   }

   public List<RoomTypeModel> GetAvailableRoomTypes( DateTime startDate, DateTime endDate )
   {
      sql = @"SELECT t.Id, t.Title, t.Description, t.Price
            FROM Rooms r
            INNER JOIN RoomTypes t ON t.Id = r.RoomTypeId
            WHERE r.Id NOT IN (
            SELECT b.RoomId FROM Bookings b
            WHERE ( @startDate < b.StartDate AND @endDate > b.EndDate )
               OR ( b.StartDate <= @endDate AND @endDate < b.EndDate )
               OR ( b.StartDate <= @startDate AND @startDate < b.EndDate ))
               GROUP BY t.id, t.Title, t.Description, t.Price;";

      var output =  _db.LoadData<RoomTypeModel, dynamic>( sql,
                                           new { startDate, endDate },
                                           connectionStringName );
      
      output.ForEach(x => x.Price = x.Price / 100);
      
      return output;
   }


   public RoomTypeModel GetRoomTypeById( int id )
   {
      sql = @"SELECT [Id], [Title], [Description], [Price]
              FROM RoomTypes WHERE Id = @id;";

      return _db.LoadData<RoomTypeModel, dynamic>( sql,
                                                  new { id },
                                                  connectionStringName).FirstOrDefault();
   }

   public List<BookingFullModel> SearchBookings( string lastName )
   {
      sql = @"SELECT [b].[Id], [b].[RoomId], [b].[GuestId], [b].[StartDate], [b].[EndDate], [b].[CheckedIn], 
                     [b].[TotalCost], [g].[FirstName], [g].[LastName], [r].[RoomNumber], [r].[RoomTypeId],
                     [rt].[Title], [rt].[Description], [rt].[Price]
              FROM Bookings b INNER JOIN Guests g ON b.GuestId = g.Id 
                                  INNER JOIN Rooms r ON b.RoomId = r.Id 
                                  INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id   
              WHERE b.StartDate = @startDate AND g.LastName Like '%' || @lastName || '%';";

      var output = _db.LoadData<BookingFullModel, dynamic>( sql,
                                                     new { lastName, startDate = DateTime.Now.Date },
                                                     connectionStringName);

      output.ForEach( x => {
         x.Price = x.Price / 100;
         x.TotalCost = x.TotalCost / 100;
      });

      return output;
   }
}
