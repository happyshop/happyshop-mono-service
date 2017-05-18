using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using HappyShop.Database;
using HappyShop.Model;
using HappyShop.ServiceConnector;
using log4net;

namespace HappyShop.WebService
{
  public class Service : IService
  {
    private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static readonly Func<bool, KioskDatabase> KioskDatabase = withTransaction => new KioskDatabase(() => new KioskConnection(), withTransaction);

    public readonly Func<KioskDatabase> KioskDatabaseWithTransaction = () => KioskDatabase(true);
    public readonly Func<KioskDatabase> KioskDatabaseWithoutTransaction = () => KioskDatabase(false);
    
    private Item ItemByBarcode(string barcode)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction())
      {
        return kioskDatabase.SelectItemByBarcode(barcode);
      }
    }

    private User UserByShortname(string shortname)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction())
      {
        return kioskDatabase.SelectUserByShortname(shortname);
      }      
    }
    
    public Response GetUserByShortname(string shortname)
    {
      User user = UserByShortname(shortname);
      return user == null ? new Response {ReturnCode = ResponseReturnCode.UserNotFound} : new Response {ReturnCode = ResponseReturnCode.Ok, User = user};
    }

    public Response GetItemByBarcode(string barcode)
    {
      Item item = ItemByBarcode(barcode);
      return item == null ? new Response {ReturnCode = ResponseReturnCode.ItemNotFound} : new Response {ReturnCode = ResponseReturnCode.Ok, Item = item};
    }

    public Stream GetItemImage(string barcode)
    {
      using ( KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction() )
      {
        return kioskDatabase.GetItemImageStream(barcode);
      }
    }

    public Response OrderItem(String shortname, String barcode)
    {
      return OrderItemWithOrigin(shortname, barcode, "0");
    }

    public Response OrderItemWithOrigin(String shortname, String barcode, String origin)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithTransaction())
      {
        Item item = kioskDatabase.SelectItemByBarcode(barcode);
        if (item == null)
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.ItemNotFound,
            Message = string.Format("Hi {0}! The barcode '{1}' could not be found in the data base.", shortname, barcode)
          };
        }

        if (kioskDatabase.AddPurchase(shortname, item, origin))
        {
          kioskDatabase.Commit();

          Item updatedItem = ItemByBarcode(barcode);
          User updatedUser = UserByShortname(shortname);
          return new Response
          {
            ReturnCode = ResponseReturnCode.Ok,
            Item = updatedItem,
            User = updatedUser,
            Message = string.Format("Dear {0} {1}! Thanks for ordering '{2}'.", updatedUser.Name, updatedUser.Lastname, updatedItem.Description)
          };
        }

        return new Response
        {
          ReturnCode = ResponseReturnCode.FailedToAddPurchase,
          Message = string.Format("Hi {0}! Failed to purchase item '{1}'.", shortname, item.Description)
        };
      }
    }

    public Response UpdateStock(string barcode, string count)
    {
      return UpdateStockWithOrigin(barcode, count, "0");
    }

    public Response UpdateStockWithOrigin(string barcode, string count, string origin)
    {
      Log.Info(string.Format("UpdateStock({0}, {1}) called.", barcode, count));
      using (KioskDatabase kioskDatabase = KioskDatabaseWithTransaction())
      {
        Log.Info("Before TryParse.");
        int iCount;
        if (!int.TryParse(count, NumberStyles.Number, CultureInfo.InvariantCulture, out iCount))
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.FailedToParseCount,
            Message = string.Format("Hi stranger! Failed to parse count '{0}'.", count)
          };
        }

        Log.Info("Before calling UpdateStock() on kiosk database.");
        if (kioskDatabase.UpdateStock(barcode, iCount, origin))
        {
          Log.Info("Before commit.");
          kioskDatabase.Commit();
          Log.Info("Before ItemByBarcode.");
          Item updatedItem = ItemByBarcode(barcode);
          return new Response
          {
            ReturnCode = ResponseReturnCode.Ok,
            Item = updatedItem,
            Message =
              string.Format("Dear stranger! You successfully updated stock for item '{0}'.", updatedItem.Description)
          };
        }

        return new Response
        {
          ReturnCode = ResponseReturnCode.FailedToParseCount,
          Message = string.Format("Dear stranger! Failed to update stock for barcode '{0}'.", barcode)
        };
      }
    }

    public Response CancelLastOrder(string shortname)
    {
      return CancelLastOrderWithOrigin(shortname, "0");
    }

    public Response CancelLastOrderWithOrigin(string shortname, string origin)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithTransaction())
      {
        User user = UserByShortname(shortname);
        if (user == null)
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.UserNotFound,
            Message = string.Format("Hi stranger! The user '{0}' could not be found in the data base.", shortname)
          };
        }

        if (kioskDatabase.CancelLastOrder(shortname, origin))
        {
          kioskDatabase.Commit();

          User updatedUser = UserByShortname(shortname);
          return new Response
          {
            ReturnCode = ResponseReturnCode.Ok,
            User = updatedUser,
            Message = string.Format("Hi {0} {1}! Your last order has been canceled. See you next time at the kiosk.", updatedUser.Name, updatedUser.Lastname)
          };
        }

        return new Response
        {
          ReturnCode = ResponseReturnCode.FailedToCancelLastOrder,
          Message = string.Format("Hi {0} {1}! Failed to cancel your last order.", user.Name, user.Lastname)
        };
      }
    }

    public string[] GetItemIds()
    {
      using ( KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction() )
      {
        return kioskDatabase.Items(null).Select(item => item.Barcode).ToArray();
      }
    }

    public Response PayIn(string shortname, string amount)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithTransaction())
      {
        float fAmount;
        if (!float.TryParse(amount, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out fAmount))
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.FailedToParseAmount,
            Message = string.Format("Hi stranger! Failed to parse amount '{0}'.", amount)
          };
        }

        User user = UserByShortname(shortname);
        if (user == null)
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.UserNotFound,
            Message = string.Format("Hi stranger! The user '{0}' could not be found in the data base.", shortname)
          };
        }

        if (kioskDatabase.PayIn(shortname, fAmount))
        {
          kioskDatabase.Commit();

          User updatedUser = UserByShortname(shortname);
          return new Response
          {
            ReturnCode = ResponseReturnCode.Ok,
            User = updatedUser,
            Message =
              string.Format("Hi {0} {1}! Thanks for paying in {2} EUR.", updatedUser.Name, updatedUser.Lastname,
                fAmount.ToString("0.00", CultureInfo.InvariantCulture))
          };
        }

        return new Response
        {
          ReturnCode = ResponseReturnCode.FailedToPayIn,
          Message =
            string.Format("Dear {0} {1}! Failed to pay in {2} EUR. Sorry!", user.Name, user.Lastname,
              fAmount.ToString("0.00", CultureInfo.InvariantCulture))
        };
      }
    }

    public Response SendBalance(string shortname)
    {
      User user = UserByShortname(shortname);
      if (user == null)
      {
        return new Response
        {
          ReturnCode = ResponseReturnCode.UserNotFound,
          Message = string.Format("Hi stranger! The user '{0}' could not be found in the data base.", shortname)
        };
      }
        
      new SimpleSmtpClient("Kiosk account information", user.Mail)
        .AddBody("Dear {0} {1}!\r\n", user.Name, user.Lastname)
        .AddBody("Your kiosk account balance is {0} EUR.", user.Balance.ToString("F2", CultureInfo.InvariantCulture))
        .Send();

      return new Response
      {
        ReturnCode = ResponseReturnCode.Ok,
        Message = string.Format("Hi {0} {1}! Balance is sent. Please check your inbox.", user.Name, user.Lastname)
      };
    }

    public Response AddItems(string barcode, string count, string fullPrice)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithTransaction())
      {
        float fFullPrice;
        if (!float.TryParse(fullPrice, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out fFullPrice))
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.FailedToParseFullPrice,
            Message = string.Format("Hi stranger! Failed to parse full price.", fullPrice)
          };
        }

        int iCount;
        if (!int.TryParse(count, NumberStyles.Number, CultureInfo.InvariantCulture, out iCount))
        {
          return new Response
          {
            ReturnCode = ResponseReturnCode.FailedToParseCount,
            Message = string.Format("Hi stranger! Failed to parse count.", count)
          };
        }

        if (kioskDatabase.AddItem(barcode, iCount, fFullPrice))
        {
          kioskDatabase.Commit();

          Item updatedItem = ItemByBarcode(barcode);
          return new Response
          {
            ReturnCode = ResponseReturnCode.Ok,
            Item = updatedItem,
            Message = string.Format("Hi stranger! {0} items of '{1}' were stocked.", iCount, updatedItem.Description)
          };
        }

        return new Response
        {
          ReturnCode = ResponseReturnCode.FailedToAddItems,
          Message = string.Format("Dear stranger! Failed to add items to the database.")
        };
      }
    }

    public Response SendBalanceToAllUsers()
    {
      // todo: walk thru 'users' and send mails with balance
      return new Response { ReturnCode = ResponseReturnCode.NotImplemented, Message = "Hi stranger! The programmer was too lazy to implement this method." };
    }

    public Response SendSummaryToManagement()
    {
      new SimpleSmtpClient("this is a test email.", "kiosk@mailserver.com")
        .AddBody("this is my test email body")
        .Send();
      return new Response { ReturnCode = ResponseReturnCode.NotImplemented, Message = "Hi stranger! The programmer was too lazy to implement this method." };
    }

    public IEnumerable<User> GetAllUsers(string filter)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction())
      {
        return kioskDatabase.Users(filter);
      }
    }

    public IEnumerable<Item> GetAllItems(string filter)
    {
      using (KioskDatabase kioskDatabase = KioskDatabaseWithoutTransaction())
      {
        return kioskDatabase.Items(filter);
      }
    }
  }
}
