CREATE TABLE dbo.GroundRoutes
(
    Id uniqueidentifier NOT NULL
        CONSTRAINT PK_GroundRoutes PRIMARY KEY,
    RouteData nvarchar(max) NOT NULL,
    CONSTRAINT CK_GroundRoutes_RouteData_IsJson
        CHECK (ISJSON(RouteData) = 1)
);
