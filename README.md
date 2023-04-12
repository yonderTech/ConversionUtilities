# ConversionUtilities

**EntityMapperService**

The service handles conversion of an object (clones it actually) to another type, even if _IConvertible _is not implemented.

You may not want database access objects (Entity Framework models, for example) to be directly exposed as an API response to the client, so you create a DTO (data transfer object) whose definition only expose the data access object' properties you want.

Converting from DAO to DTO can be a daunting task, with EntityMapperService, not anymore.

`EntityMapperService service = new EntityMapperService();`

`DAO.A daoA = new DAO.A { /* Whatever data the object is instanciated with */  };`

`Dto.A dtoA = service.Clone<Dto.A>(daoA);`

There it is, that simple. The method assumes that property names and types and of the same on both sides.

