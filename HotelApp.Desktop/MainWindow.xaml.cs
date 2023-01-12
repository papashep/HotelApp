using HotelAppLibrary.Data;
using HotelAppLibrary.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.Desktop;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
   private readonly IDatabaseData _db;

   public MainWindow(IDatabaseData db)
   {
      InitializeComponent();
      _db = db;
   }

   private void searchForGuest_Click( object sender, RoutedEventArgs e )
   {
      List<BookingFullModel> bookings = _db.SearchBookings(lastNameText.Text );
      resultsList.ItemsSource = bookings;
   }

   private void checkInButton_Click( object sender, RoutedEventArgs e )
   /*
      'e' is the routed events arguments and contains inside the Source is the DataContext and inside that
          is a copy of the Models.BookingFullModel in the HotelAppLibrary containg all the booking data.
          So if we say e.Source.DataContext that will be the BookingFullModel data for the person we 
          clicked on the Check In button.
            1. var model = e.Source.DataContext.();
            2. Cast this to type BookingFullModel  
               - var model = (BookingFullModel)e.Source.DataContext.();
            3. Change to cast Button, Wrap it in parens
               - var model = ((Button)e.Source).DataContext;
            4. Finnaly cast this to type BookingFullModel
               - var model = (BookingFullModel)((Button)e.Source).DataContext;
          We had to do a couple of different casts because it didn't know that e.Source was a button.
          Then the DataContext thinks it's an object, so we actually cast it BookingFullModel because they are the same.
          Now we can use BookingFullModel model which has all the information we need.

          How do we get this model into our CheckInForm
   */
   {
      var checkInForm = App.serviceProvider.GetService<CheckInForm>();
      if ( checkInForm != null )
      {
         var model = (BookingFullModel)((Button)e.Source).DataContext;
         checkInForm.PopulateCheckInInfo( model );

         checkInForm.Show();
      }

   }
}
