using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using HappyShop.Model;

namespace HappyShop.WebService
{
  [ServiceContract]
  public interface IService
  {
    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response GetUserByShortname(String user);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/item/{barcode}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response GetItemByBarcode(String barcode);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/item/{barcode}/image", RequestFormat = WebMessageFormat.Json)]
    Stream GetItemImage(string barcode);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/order/{barcode}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response OrderItem(String user, String barcode);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/order/{barcode}/origin/{origin}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response OrderItemWithOrigin(String user, String barcode, String origin);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/item/{barcode}/newCount/{count}/origin/{origin}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response UpdateStockWithOrigin(String barcode, String count, String origin);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/cancelLastOrder", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response CancelLastOrder(String user);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/cancelLastOrder/origin/{origin}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response CancelLastOrderWithOrigin(String user, String origin);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/itemids", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    string[] GetItemIds();

    /// <summary>
    /// A user pays in an amount to his/her account
    /// </summary>
    /// <param name="user">The short code of the user.</param>
    /// <param name="amount">The amount to pay in.</param>
    /// <returns>Response object</returns>
    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/payin/{amount}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response PayIn(String user, String amount);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/user/{user}/sendBalance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response SendBalance(String user);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/item/{barcode}/add/{count}/fullPrice/{fullPrice}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response AddItems(String barcode, String count, String fullPrice);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/sendBalanceToAllUsers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response SendBalanceToAllUsers();

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/sendSummaryToManagement", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    Response SendSummaryToManagement();

    // management services

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/users?filter={filter}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    IEnumerable<User> GetAllUsers(String filter);

    [OperationContract]
    [WebGet(UriTemplate = "kiosk/items?filter={filter}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    IEnumerable<Item> GetAllItems(String filter);

    //[OperationContract]
    //[WebInvoke(Method = "POST", UriTemplate = "kiosk/adduser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void PopulateUser();

    //[OperationContract]
    //[WebInvoke(Method = "POST", UriTemplate = "kiosk/additem", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void PopulateItem();

    //[OperationContract]
    //[WebInvoke(Method = "POST", UriTemplate = "kiosk/addusers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void PopulateUsers();

    //[OperationContract]
    //[WebInvoke(Method = "POST", UriTemplate = "kiosk/additems", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void PopulateItems();

    //[OperationContract]
    //[WebInvoke(Method = "DELETE", UriTemplate = "kiosk/deleteuser/{user}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void DeleteUser(string user);

    //[OperationContract]
    //[WebInvoke(Method = "DELETE", UriTemplate = "kiosk/deleteitem/{item}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void DeleteItem(string barcode);

    //[OperationContract]
    //[WebInvoke(Method = "DELETE", UriTemplate = "kiosk/deleteusers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void DeleteUsers();

    //[OperationContract]
    //[WebInvoke(Method = "DELETE", UriTemplate = "kiosk/deleteitems", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
    //void DeleteItems();

  }
}
