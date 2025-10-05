using Microsoft.AspNetCore.Http;

namespace E_Shop.Core.Abstractions;

public static class Errors
{
    public class UserErrors
    {
        public static readonly Error InvalidCredentionals =
            new Error("User.InvalidCredentionals", "Invalid Username or password", StatusCodes.Status401Unauthorized);
        public static readonly Error DuplicateEmail =
            new Error("User.DuplicatedEmail", "Another User with the same Email", StatusCodes.Status409Conflict);
        public static readonly Error EmailNotConfirmed =
            new Error("User.EmailNotConfirmed", "EmailNotConfirmed", StatusCodes.Status401Unauthorized);
        public static readonly Error DuplicateConfirmation =
            new Error("User.DuplicateConfirmation", "This email had been confirmed before", StatusCodes.Status409Conflict);
        public static readonly Error NotFound =
            new Error("User.NotFound", "This Id is not valid", StatusCodes.Status404NotFound);
        public static readonly Error InvalidCode =
            new Error("User.InvalidCode", "InvalidCode", StatusCodes.Status400BadRequest);
        public static readonly Error UserDisabled =
            new Error("User.DisabledUser", "This user is disabled call the adminsterator", StatusCodes.Status409Conflict);
        public static readonly Error UserLocked =
            new Error("User.DisabledUser", "This user is locked call the adminsterator", StatusCodes.Status409Conflict);
        public static readonly Error NotAssigned =
            new Error("User.NotAssignedToRole", "This user is not Assigned To any Role", StatusCodes.Status404NotFound);

    }
    public class UserRoleErrors
    {
        public static readonly Error EmptyUserId = 
            new Error("UserRole.EmptyUserId", "UserId can not be null or empty", StatusCodes.Status400BadRequest);
        public static readonly Error EmptyRoleName =
            new Error("UserRole.EmptyRoleName", "RoleName can not be null or empty", StatusCodes.Status400BadRequest);
        public static readonly Error RoleNotFound =
            new Error("UserRole.RoleNotFound", "This Role is not Existed", StatusCodes.Status404NotFound);
        public static readonly Error UserNotFound =
            new Error("UserRole.UserNotFound", "This User is not Existed", StatusCodes.Status404NotFound);
        public static readonly Error AlreadyInRole =
            new Error("UserRole.AlreadyInRole", "This User is already assigned to this Role", StatusCodes.Status409Conflict);
        public static readonly Error NotInRole =
            new Error("UserRole.NotInRole", "This User is not assigned to this Role", StatusCodes.Status409Conflict);
    }
    public class RoleErrors
    {
        public static readonly Error DuplicateRole =
            new Error("Role.DuplicatedRole", "Another Role with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error NotFound =
            new Error("Role.NotFound", "This Role is not Existed", StatusCodes.Status404NotFound);
        public static readonly Error NotValid =
            new Error("Role.NotValid", "This Role is not valid", StatusCodes.Status409Conflict);
    }
    public class TokenErrors
    {
        public static readonly Error EmptyToken =
            new Error("NotFound", "Null Refrence", StatusCodes.Status404NotFound);
    }
    public class ProductTypeErrors
    {
        public static readonly Error NotFound =
            new Error("ProductType.NotFound", "No ProductType with this id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateName =
            new Error("ProductType.DuplicateName", "Another ProductType with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error HasProducts =
            new Error("ProductType.HasProducts", "This ProductType has Products, you can't delete it", StatusCodes.Status409Conflict);
        public static readonly Error EmptyProductType
            = new Error("ProductType.Empty", "This ProductType can not be empty", StatusCodes.Status409Conflict);
    }
    public class ProductErrors
    {
        public static readonly Error NotFound =
            new Error("Product.NotFound", "No Product with this id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateName =
            new Error("Product.DuplicateName", "Another Product with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error HasProducts =
            new Error("Product.HasProducts", "This Product has Products, you can't delete it", StatusCodes.Status409Conflict);
        public static readonly Error EmptyProduct
            = new Error("Product.Empty", "This Product can not be empty", StatusCodes.Status409Conflict);
    }
    public class ManufacturingTypeErrors
    {
        public static readonly Error NotFound =
            new Error("ManufacturingType.NotFound", "No ManufacturingType with this id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateName =
            new Error("ManufacturingType.DuplicateName", "Another ManufacturingType with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error HasProducts =
            new Error("ManufacturingType.HasProducts", "This ManufacturingType has Products, you can't delete it", StatusCodes.Status409Conflict);
        public static readonly Error EmptyManufacturingType
            = new Error("ManufacturingType.Empty", "This ManufacturingType can not be empty", StatusCodes.Status409Conflict);
    }
    public class ManufacturingCompanyErrors
    {
        public static readonly Error NotFound =
            new Error("ManufacturingCompany.NotFound", "No ManufacturingCompany with this id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateName =
            new Error("ManufacturingCompany.DuplicateName", "Another ManufacturingCompany with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error HasProducts =
            new Error("ManufacturingCompany.HasProducts", "This ManufacturingCompany has Products, you can't delete it", StatusCodes.Status409Conflict);
        public static readonly Error EmptyManufacturingCompany
            = new Error("ManufacturingCompany.Empty", "This ManufacturingCompany can not be empty", StatusCodes.Status409Conflict);
    }
    public class CustomerErrors
    {
        public static readonly Error NotFound =
            new Error("Customer.NotFound", "No Customer with this id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateName =
            new Error("Customer.DuplicateName", "Another Customer with the same Name", StatusCodes.Status409Conflict);
        public static readonly Error HasProducts =
            new Error("Customer.HasProducts", "This Customer has Products, you can't delete it", StatusCodes.Status409Conflict);
        public static readonly Error EmptyCustomer
            = new Error("Customer.Empty", "This Customer can not be empty", StatusCodes.Status409Conflict);
    }
    public class ItemErrors
    {
        public static readonly Error Emptyitem =
            new Error("Item.NotFound", "No Item with this id", StatusCodes.Status404NotFound);
    }
    public class CategoryErrors
    {
        public static readonly Error EmptyCategory =
            new Error("Category.NotFound", "No Category with this id", StatusCodes.Status404NotFound);
    }
    public class DiscountErrors
    {
        public static readonly Error InvalidPrice =
            new Error("Discount.InvalidPrice", "New Price Must be less than Old Price", StatusCodes.Status409Conflict);
        public static readonly Error InvalidDateRange =
            new Error("Discount.InvalidDateRange", "EndDate must be more than Start Date", StatusCodes.Status409Conflict);
        public static readonly Error ExistingDiscount =
            new Error("Discount.ExistingDiscount", "This item have a discount already", StatusCodes.Status409Conflict);
        public static readonly Error InvalidDiscount =
            new Error("Discount.Invalid", "No Discount Match this data", StatusCodes.Status409Conflict);
    }
    public class OfferErrors
    {
        public static readonly Error EmptyOffer =
            new Error("Offer.NotFound", "No Offer with this id", StatusCodes.Status404NotFound);
        public static readonly Error NotValidOffers =
            new Error("Offer.NotValid", "No Available Offers right now", StatusCodes.Status404NotFound);
    }
    public static class OfferItemErrors
    {
        public static readonly Error EmptyOfferItem =
            new Error("OfferItem.NotFound", "No OfferItem with this id", StatusCodes.Status404NotFound);
    }
}
