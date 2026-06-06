namespace DROWeb.Domain.Models;

[System.Flags]
public enum Permission : ulong
{
    None = 0,

    Play = 1 << 0,
    Audit = 1 << 1,
    Moderate = 1 << 2,
    ManageSession = 1 << 3,

    Testing = 1 << 10,

    SystemBypass = 1UL << 29,
    ManagePermissions = 1 << 30,

    RoleAll = ulong.MaxValue
}
