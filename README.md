# Data Library
This library will help you define the data your application utilizes. It will also help you define a common interface your application will use when accessing or manipulating data objects in a database or a remote server.

## Data Object
A data object is used to represent data accessed through the data layer and used by your application. For example, your application displays banking accounts for a user so your application will need an Account object and it would need properties like AccountType and TotalAmount to describe the account. Using the library, you would define an Account object as such:
```
public class Account : DataObject
{
   public AccountType AccountType { get; set; }

   public Decimal TotalAmount { get; set; }

   public override MapProperties(DataObject dataObject)
   {
      base.MapProperties(dataObject);

      if (dataObject is Account account)
      {
         AccountType = account.AccountType;
         TotalAmount = account.TotalAmount;
      }
   }
}
```
The library is built on the idea you create a class which represents your data and it inherits from one of the two base classes defined in the library. Each base class describes the data in certain ways.

### Data Object Class
This contains an identifier the record in a database table or collection and there are two properties, an Integer64ID property and StringID property, and **the library expects you to choose if either one or both are used by your application and your data layers.** Also, included is a CreatedOn and LastEditedOn properties which keep track of when the data object was created and when it was lasted edited. Additionally, there are common optional properties, Name, Description, LastEditedBy (a user name), LastEditedByInteger64ID (if integer identifiers are used) and LastEditedByStringID (if string identifiers are used).

There is an overridable MapProperties() method which you can override it in your subclasses to define how data is copied from one data object to another. There is also a Validate() method which can be used by the data layer to validate the data of the object; this uses data annotations.

#### Data Annotations
If you need to make Name required or apply any other data annotation attributes to the base properties, you can override them; they're all virtual.
```
public class Account : DataObject
{
   AccountType AccountType { get; set; }

   public Decimal TotalAmount { get; set; }

   //I overridden the base to include a Required data annotation attribute.
   //The base.Validate() will respect this.
   [Required]
   public override string? Name { get => base.Name; set => base.Name = value; }

   public override MapProperties(DataObject dataObject)
   {
      base.MapProperties(dataObject);

      if (dataObject is Account account)
      {
         AccountType = account.AccountType;
         TotalAmount = account.TotalAmount;
      }
   }
}
```

### Sub Data Object Class
This represents data that's relationally under something else; think *this* has *that* and *that* would be the sub of *this*. Like the DataObject class, it has two owner identifier properties, OwnerInteger64ID and OwnerStringID, and these will reference an owner data object. **The library expects you to choose if either one or both are used by your application and your data layers.**

Going back to the banking example, an account will have transactions taken by the user or externally (auto deposit/withdrawl) so that's something your application would need to display. Your application would need a Transaction object and you would need to set the owner identifier property to the identifier of the Account object. This defines a relationship between the Account object and Transaction object.
```
public class Transaction : SubDataObject
{
   public ActionType ActionTaken { get; set; }

   public Decimal Amount { get; set; }

   public override MapProperties(DataObject dataObject)
   {
      base.MapProperties(dataObject);

      if (dataObject is Transaction transaction)
      {
         ActionTaken = transaction.ActionTaken;
         Amount = transaction.Amount;
      }
   }
}

//Somewhere in your application you would create a new transaction and
//bind it to the account the transaction was taken on with the OwnerInteger64ID.
await dataLayer.CreateAsync(new Transaction()
{
   ActionTaken = ActionType.Deposit,
   Amount = 10.50,
   OwnerInteger64ID = accountDataObject.Integer64ID,
});
```

### List View Class
This is not a data object but its derived from the DataObject or SubDataObject. A list view only contains a name and identifier and its meant for a dropdown or similar UI where it only needs the name and identifier and nothing else.

## Data Layer
Your application will use the data layer to access or manipulate data objects. The data layer will act as a wrapper for the database or a remote server. This abstraction allows your application to only know about the data objects and the data layers and not how its needs to talk to the source.

This library defines two types of data layers, database and HTTP. Both have interfaces that define common data access/manipulation. For the database side, this library only contains memory storage data layers and these are meant more for prototyping or example projects. For the HTTP side, this library contains HTTP data layers meant for communicating with a web API and these use the System.Net.Http.HttpClient class.

The data layer classes in the library are generic. The library is built on the idea you create a sub data layer class from whichever base data layer class you need and define which data object the data layer will represent. This creates a strongly typed association between the data object and data layer.

### Memory Storage Data Layer
**First, let's start off by saying the memory storage data layer uses an auto incrementing long identity as the identifier so this means your application needs to use the DataObject.Integer64ID property when interacting with a data object and its data layer.** 

#### How to Create Your Memory Storage Data Layer 
Going back to our banking account example, your application needs a way to interact with the account data so you need to define an interface for your account data layer and an account data layer sub class which inherits from StandardCRUDDataLayer in the JMayer.Data.Database.DataLayer.MemoryStorage namespace. Optionally, the data layer would need to be registered to the middleware in the Program.cs; this allows for depedency injection if your application utilizes it.
```
public interface IAccountDataLayer : IStandardCRUDDataLayer<Account>
{
}

public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
}

//Optionally, register the data layer with the middleware in Program.cs.
builder.Services.AddSingleton<IAccountDataLayer, AccountDataLayer>();
```
You now have an account data layer which your application can interact with and it contains a standard set of ways to retrieve or manipulate account data objects. The same can be done when using the StandardSubCRUDDataLayer.

#### How to Create a New Data Object Using Your Memory Storage Data Layer
```
Account account = await accountDataLayer.CreateAsync(new Account()
{
   AccountType = AccountType.Checking,
   TotalAmount = 100.00,
});
```
The CreateAsync() method can take a single object or a list. ValidateAsync() will be called for each data object; base functionality checks the data annotations on the object but the ValidateAsync() method can be overridden to add additional validation checks. Any validation issue will throw a DataObjectValidationException. The Created event will also be called after all the data objects have been inserted into the list. If your application needs to react to the creation in some way, you can register an event handler with the Created event.

#### How to Delete a Data Object Using Your Memory Storage Data Layer
```
await accountDataLayer.DeleteAsync(account);
//or
await accountDataLayer.DeleteAsync(obj => obj.AccountType == AccountType.Savings);
```
The DeleteAsync() method can take a single object or a list. It can also take an expression. The Deleted event will be call after all the data objects have been deleted from the list. If your application needs to react to the deletion in some way, you can register an event handler with the Deleted event.

#### How to Update a Data Object Using Your Memory Storage Data Layer
```
account.TotalAmount = 250.00;
account = await accountDataLayer.UpdateAsync(account);
```
The UpdateAsync() method can take a single object or a list. ValidateAsync() will be called for each data object; base functionality checks the data annotations on the object but the ValidateAsync() method can be overridden to add additional validation checks. Any validation issue will throw a DataObjectValidationException. Additionally, the existence of each data object will be confirmed in the list (this is before the list is update) and if any are missing, an DataObjectIDNotFoundException will be thrown. The Updated event will also be called after all the data objects have been updated in the list. If your application needs to react to the update in some way, you can register an event handler with the Updated event.

#### How to Get Data Objects Using Your Memory Storage Data Layer
```
var accounts = await accountDataLayer.GetAllAsync();
//or
var accounts = await accountDataLayer.GetAllAsync(wherePredicate: obj => obj.TotalAmount > 10000);
//or
var accounts = await accountDataLayer.GetAllAsync(wherePredicate: obj => obj.TotalAmount > 10000, orderByPredicate: obj => obj.TotalAmount, descending: true);
//or
var accounts = await accountDataLayer.GetAllAsync(orderByPredicate: obj => obj.TotalAmount, descending: true);
//
var accounts = await accountDataLayer.GetAllListViewAsync();
//or
var accounts = await accountDataLayer.GetAllListViewAsync(wherePredicate: obj => obj.TotalAmount > 10000);
//or
var accounts = await accountDataLayer.GetAllListViewAsync(wherePredicate: obj => obj.TotalAmount > 10000, orderByPredicate: obj => obj.TotalAmount, descending: true);
//or
var accounts = await accountDataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.TotalAmount, descending: true);
```
You can pass in an expression for the where predicate. You can pass in an expression for the order by predicate. You can also pass in if the order by is descending.

#### How to Get Paged Data Objects Using Your Memory Storage Data Layer
```
//Skip 3 pages and then, take 10 accounts.
QueryDefinition queryDefinition = new()
{
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Filter for savings accounts.
QueryDefinition queryDefinition = new()
{
    FilterDefinitions =
    [
       new FilterDefinition()
       {
          FilterOn = nameof(Account.AccountType),
          Operator = FilterDefinition.EqualsOperator,
          Value = AccountType.Savings,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Accounts ordered descending by total amount.
QueryDefinition queryDefinition = new()
{
    SortDefinitions =
    [
       new SortDefinition()
       {
          SortOn = nameof(Account.TotalAmount),
          Descending = true,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Skip 3 pages and then, take 10 accounts as list views.
QueryDefinition queryDefinition = new()
{
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
//or
//Filter for savings accounts as list views.
QueryDefinition queryDefinition = new()
{
    FilterDefinitions =
    [
       new FilterDefinition()
       {
          FilterOn = nameof(Account.AccountType),
          Operator = FilterDefinition.EqualsOperator,
          Value = AccountType.Savings,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
//or
//Accounts as list views ordered descending by total amount.
QueryDefinition queryDefinition = new()
{
    SortDefinitions =
    [
       new SortDefinition()
       {
          SortOn = nameof(Account.TotalAmount),
          Descending = true,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
```
This will return a page of data objects and it accepts a QueryDefinition object. The QueryDefinition defines how to filter and/or sort, how many data objects to take and what page of data objects to return.

#### How to Get a Single Data Object Using Your Memory Storage Data Layer
```
var account = await accountDataLayer.GetSingleAsync();
//or
var account = await accountDataLayer.GetSingleAsync(obj => obj.Integer64ID == 10);
```
You can pass in an expression for the where predicate. Either call will do a FirstOrDefault() method call.

#### How to Enable Name Uniqueness Check
```
public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public AccountDataLayer() => IsUniqueNameRequired = true
}
```
IsUniqueNameRequired can only be set during object creation (constructor or object initializer). When its set to true, on CreateAsync() or UpdateAsync(), the base ValidateAsync() will check the Name property is unique compared to the other data objects in memory storage.

#### How to Enable Old Data Object Check
```
public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public AccountDataLayer() => IsOldDataObjectDetectionEnabled = true
}
```
IsOldDataObjectDetectionEnabled can only be set during object creation (constructor or object initializer). When its set to true, on UpdateAsync(), the LastEditedOn property will be used to determine if the data object being passed in isn't old. If the data object in memory storage has a newer timestamp, a DataObjectUpdateConflictException is thrown.

#### How to Expand Your Memory Storage Data Layer
Now, let's say you need to add additional functionality to the account data layer and you can easy do so by adding a new method to the IAccountDataLayer interface and to the AccountDataLayer class.
```
public interface IAccountDataLayer : IStandardCRUDDataLayer<Account>
{
   Task<List<Account>> GetSavingAccountsAsync()
}

public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public async Task<List<Account>> GetSavingAccountsAsync()
   {
      var savingAccounts = base.QueryData(wherePredicate: obj => obj.AccountType == AccountType.Savings);
      return Task.FromResult(savingAccounts);
   }
}
```
The base has a QueryData() method you can call; you can either pass in a QueryDefinition object or linq for a where predicate and/or linq for an order predicate and/or bool for ascending/descending order. If for some reason, you need direct access to the list, it can be accessed with the DataStorage property but it will require a lock(DataStorageLock) { ... } statement in order to maintain thread-safety. If for some reason, the CreateAsync() method needs to be overridden, to access the identity, use the Identity property and to increment the identity use the IncrementIdentity() method; again, in order to maintain thread-safety, calling either will require a lock(DataStorageLock) { ... } statement.

#### How to Override Base Functionality in Your Memory Storage Data Layer
Let's also say you need to do something extra in the CreateAsync() method. It's easy because the methods are virtual so you can override them.
```
public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public override async Task<Account> CreateAsync(Account dataObject, CancellationToken cancellationToken = default)
   {
      dataObject = await base.CreateAsync(dataObject, cancellationToken);
      //Do your extra thing with the data object.
      return dataObject;
   }
}
```

### HTTP Data Layer
**First, let's start off by saying the HTTP data layer is tightly coupled with the web API controllers defined in the JMayer.Web.Mvc library. If you're not using the JMayer.Web.Mvc library then your web API controllers will need to match the expected HTTP request and response defined here.**

#### How to Create Your HTTP Data Layer 
Going back to our banking account example, your application needs a way to interact with the account data so you need to define an interface for your account data layer and an account data layer sub class which inherits from StandardCRUDDataLayer in the JMayer.Data.HTTP.DataLayer namespace. Optionally, the data layer would need to be registered to the middleware in the Program.cs; this allows for depedency injection if your application utilizes it.
```
public interface IAccountDataLayer : IStandardCRUDDataLayer<Account>
{
}

public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
}

//Optionally, register the data layer with the middleware in Program.cs.
//The base addresss will need to be assigned; this is how you would do it in WebAssembly Blazor.
builder.Services.AddSingleton<IAccountDataLayer, AccountDataLayer>(httpClient => httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
```
You now have an account data layer which your application can interact with and it contains a standard set of ways to retrieve or manipulate account data objects using a web API. The same can be done when using the StandardSubCRUDDataLayer.

#### How to Create a New Data Object Using Your HTTP Data Layer
```
OperationResult operationResult = await accountDataLayer.CreateAsync(new Account()
{
   AccountType = AccountType.Checking,
   TotalAmount = 100.00,
});

if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Account account)
{
   //Do something with the newly created data object.
}
else if (operationResult.IsSuccessStatusCode is false)
{
   if (operationResult.ValidationErrors.Count > 0)
   {
      //Display the validation failures.
   }
   else if (operationResult.ProblemDetails is not null)
   {
      //Display an error message returned by the server.
   }
}
```
This will send a POST HTTP request to the web API and it will contain the data object in the body as json. The Uri will be formatted as {BaseAddress}/api/{TypeName} and the TypeName would be Account in our example.

#### How to Delete a Data Object Using Your HTTP Data Layer
```
OperationResult operationResult = await accountDataLayer.DeleteAsync(account);

if (operationResult.IsSuccessStatusCode)
{
   //Do something on successful deletion.
}
else if (operationResult.IsSuccessStatusCode is false && operationResult.ProblemDetails is not null)
{
   //Display an error message returned by the server.
}
```
This will send a DELETE HTTP request to the web API. The Uri will be formatted as {BaseAddress}/api/{TypeName}/{Integer64ID} or {BaseAddress}/api/{TypeName}/{StringID} and the TypeName would be Account in our example.

#### How to Update a Data Object Using Your HTTP Data Layer
```
account.TotalAmount = 250.00;
OperationResult operationResult = await accountDataLayer.UpdateAsync(account);

if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Account account)
{
   //Do something with the updated data object.
}
else if (operationResult.IsSuccessStatusCode is false)
{
   if (operationResult.ValidationErrors.Count > 0)
   {
      //Display the validation failures.
   }
   else if (operationResult.ProblemDetails is not null)
   {
      //Display an error message returned by the server.
   }
}
```
This will send a PUT HTTP request to the web API and it will contain the data object in the body as json. The Uri will be formatted as {BaseAddress}/api/{TypeName} and the TypeName would be Account in our example.

#### How to Get Data Objects Using Your HTTP Data Layer
```
var accounts = await accountDataLayer.GetAllAsync();
//or
var accountListViews = await accountDataLayer.GetAllListViewAsync();
```
This will send a GET HTTP request to the web API. The Uri will be formatted as {BaseAddress}/api/{TypeName}/All or {BaseAddress}/api/{TypeName}/All/ListView and the TypeName would be Account in our example.

The StandardSubCRUDDataLayer has a GetAllAsync() and GetAllListViewAsync() method which accepts an owner ID; the idea is the web API returns all sub data objects under an owner. The Uri will be formatted as {BaseAddress}/api/{TypeName}/All/{OwnerInteger64ID} or {BaseAddress}/api/{TypeName}/All/{OwnerStringID} or {BaseAddress}/api/{TypeName}/All/ListView/{OwnerInteger64ID} or {BaseAddress}/api/{TypeName}/All/ListView/{OwnerStringID}.

#### How to Get Paged Data Objects Using Your HTTP Data Layer
```
//Skip 3 pages and then, take 10 accounts.
QueryDefinition queryDefinition = new()
{
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Filter for savings accounts.
QueryDefinition queryDefinition = new()
{
    FilterDefinitions =
    [
       new FilterDefinition()
       {
          FilterOn = nameof(Account.AccountType),
          Operator = FilterDefinition.EqualsOperator,
          Value = AccountType.Savings,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Accounts ordered descending by total amount.
QueryDefinition queryDefinition = new()
{
    SortDefinitions =
    [
       new SortDefinition()
       {
          SortOn = nameof(Account.TotalAmount),
          Descending = true,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageAsync(queryDefinition);
//or
//Skip 3 pages and then, take 10 accounts as list views.
QueryDefinition queryDefinition = new()
{
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
//or
//Filter for savings accounts as list views.
QueryDefinition queryDefinition = new()
{
    FilterDefinitions =
    [
       new FilterDefinition()
       {
          FilterOn = nameof(Account.AccountType),
          Operator = FilterDefinition.EqualsOperator,
          Value = AccountType.Savings,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
//or
//Accounts as list views ordered descending by total amount.
QueryDefinition queryDefinition = new()
{
    SortDefinitions =
    [
       new SortDefinition()
       {
          SortOn = nameof(Account.TotalAmount),
          Descending = true,
       }
    ],
    Skip = 3,
    Take = 10,
};
PagedList pagedList = accountDataLayer.GetPageListViewAsync(queryDefinition);
```
This will send a GET HTTP request to the web API. The Uri will be formatted as {BaseAddress}/api/{TypeName}/Page?{queryDefinition} or {BaseAddress}/api/{TypeName}/Page/ListView?{queryDefinition} and the TypeName would be Account in our example. The query definition will be in the query string of the Uri and it'll be formatted as Skip={Skip}&Take={Take}&FilterDefinition[X].FilterOn={FilterOn}&FilterDefinition[X].Operator={Operator}&FilterDefinition[X].Value={Value}&SortDefinition[X].SortOn={SortOn}&SortDefinition[X].Descending={Descending}. X will be 0 to N; based on the number FilterDefinitions or SortDefinitions in each list.

The StandardSubCRUDDataLayer has a GetPageAsync() and GetPageListViewAsync() method which accepts an owner ID; the idea is the web API returns all sub data objects under an owner. The Uri will be formatted as {BaseAddress}/api/{TypeName}/Page/{OwnerInteger64ID}?{queryDefinition} or {BaseAddress}/api/{TypeName}/Page/{OwnerStringID}?{queryDefinition} or {BaseAddress}/api/{TypeName}/Page/ListView/{OwnerInteger64ID}?{queryDefinition} or {BaseAddress}/api/{TypeName}/Page/ListView/{OwnerStringID}?{queryDefinition}.

#### How to Get a Single Data Object Using Your HTTP Data Layer
```
//Query the first account stored on the server.
var account = await accountDataLayer.GetSingleAsync();
//or
//Query a specific account stored on the server.
var account = await accountDataLayer.GetSingleAsync(10);
```
This will send a GET HTTP request to the web API. The Uri will be formatted as {BaseAddress}/api/{TypeName}/Single or {BaseAddress}/api/{TypeName}/Single/{Integer64ID} or {BaseAddress}/api/{TypeName}/Single/{StringID} and the TypeName would be Account in our example.

#### How to Expand Your HTTP Data Layer
Now, let's say you need to add additional functionality to the account data layer and you can easy do so by adding a new method to the IAccountDataLayer interface and to the AccountDataLayer class.
```
public interface IAccountDataLayer : IStandardCRUDDataLayer<Account>
{
   Task<List<Account>> GetSavingAccountsAsync()
}

public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public async Task<List<Account>> GetSavingAccountsAsync()
   {
      List<Account>? dataObjects = [];
      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All/{AccountType.Savings}", cancellationToken);
      
      if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
      {
         dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<Account>?>(cancellationToken);
      }
      
      return dataObjects;
   }
}
```
Use the base HttpClient property to make calls to the web API and the base TypeName property when formatting the route. The same can be done when using the StandardSubCRUDDataLayer.

#### How to Override Base Functionality in Your HTTP Data Layer
Let's also say you need to send xml instead of json in the CreateAsync() method. It's easy because the methods are virtual so you can override them.
```
public class AccountDataLayer : StandardCRUDDataLayer<Account>, IAccountDataLayer
{
   public override async Task<Account> CreateAsync(Account dataObject, CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(dataObject);

      //Post as xml.
      HttpResponseMessage httpResponseMessage = await HttpClient.PostAsXmlAsync($"api/{TypeName}", dataObject, cancellationToken);
      
      if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
      {
          Account? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<Account?>(cancellationToken: cancellationToken);
          return new OperationResult(httpResponseMessage.StatusCode, dataObject: returnedDataObject);
      }
      else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
      {
          ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
          return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail, validationErrors: details?.Errors);
      }
      else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
      {
          ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
          return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
      }

      return new OperationResult(httpResponseMessage.StatusCode);
   }
}
```
The same can be done when using the StandardSubCRUDDataLayer.

### Build Your Own
In the real world, your application will need to interact with an actual database instead of the memory storage classes provided and currently, the JMayer library suite doesn't provide data layer classes for accessing the various databases in existence. It's also possible the HTTP data layer is too restrictive or it needs to interact with something other than a Web API. Either way, you can build your own. 

You'll need to create a data layer class for X and implement at least the IStandardCRUDDataLayer interface. Then, its about using the necessary library to communicate with the database or remote server and filling out the interface methods with functionality.

Things to be aware of on the database side:
* Expressions. Your database library will need a linq to database translator.
* Property Mappings. Your database library may need to know if Column A in Table 1 is mapped to Property A in Data Object 1.

# v9.1.0 Change Log
---
* Added a CompareToOtherMember data annotation attribute. You can compare two public members of the same class & instance and pass/fail validation based on the comparison rule you setup.
* Added a RequiredDependsOn data annotation attribute. A public member will have the Required data annotation evaulated if another public member of the same class & instance equals a boolean value or enum value.
---
* [ASP.NET Core MVC Example Project](https://github.com/jmayer913/JMayer-Example-ASPVanillaMVC)
* [ASP.NET Core MVC with Syncfusion Example Project](https://github.com/jmayer913/JMayer-Example-ASPSyncfusionMVC)
* [ASP.NET Core / React Example Project](https://github.com/jmayer913/JMayer-Example-ASPReact)
* [Blazor WebAssembly Example Project](https://github.com/jmayer913/JMayer-Example-WebAssemblyBlazor)
---
