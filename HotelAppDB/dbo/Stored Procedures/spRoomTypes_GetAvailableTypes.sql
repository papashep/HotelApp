CREATE PROCEDURE [dbo].[spRoomTypes_GetAvailableTypes]
   
   @startDate date,
   @endDate date

AS

begin

   -- Stops the number of selected items from being returned by the query
   set nocount on;

   select t.Id, t.Title, t.Description, t.Price
   from dbo.Rooms r
   inner join dbo.RoomTypes t on t.Id = r.RoomTypeId
   where r.Id not in (

   select b.RoomId
   from dbo.Bookings b
   where (@startDate < b.StartDate and @endDate > b.EndDate)
      or (b.StartDate <= @endDate and @endDate < b.EndDate)
      or (b.StartDate <= @StartDate and @startDate < b.EndDate)
   )

   group by t.id, t.Title, t.Description, t.Price;

end
