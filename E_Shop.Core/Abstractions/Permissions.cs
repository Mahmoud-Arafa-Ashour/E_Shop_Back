namespace E_Shop.Core.Abstractions;

public static class Permissions
{
    #region Properties
    public static string Type { get; set; } = "permissions";
    #endregion

    #region Permissions

    #region UserRole Controller 
    public const string Info = "info";
    public const string UpdateInfo = "UpdateInfo";
    public const string ChangePassword = "ChangePassword";
    #endregion

    #region Roles
    public const string GetRoles = "Roles:GetAll";
    public const string AddRole = "Roles:Add";
    public const string GetActiveRoles = "Roles:GetActiveRoles";
    public const string GetRoleDetails = "Roles:GetRoleDetails";
    public const string UpdateRoles = "Roles:Update";
    public const string GetRole = "Roles:Get";
    #endregion

    #region Product
    public const string GetProduct = "Product:Get";
    public const string AddProduct = "Product:Add";
    public const string UpdateProduct = "Product:Update";
    public const string DeleteProduct = "Product:Delete";
    public const string GetAllProducts = "Product:GetAll";
    #endregion

    #endregion

    #region Method
    public static IList<string?> GetAllPermissions =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
    #endregion
}
