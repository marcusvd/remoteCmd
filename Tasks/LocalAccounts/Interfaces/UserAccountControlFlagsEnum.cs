namespace remoteCmd.Tasks.LocalAccounts.Interfaces
{
  [Flags]
public enum UserFlags
{
    ADS_UF_SCRIPT = 0x0001, // The logon script is executed.
    ADS_UF_ACCOUNTDISABLE = 0x0002, // The user account is disabled.
    ADS_UF_HOMEDIR_REQUIRED = 0x0008, // The home directory is required.
    ADS_UF_LOCKOUT = 0x0010, // The account is currently locked out.
    ADS_UF_PASSWD_NOTREQD = 0x0020, // No password is required.
    ADS_UF_PASSWD_CANT_CHANGE = 0x0040, // The user cannot change the password.
    ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x0080, // The user can send an encrypted password.
    ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x0100, // This is an account for users whose primary account is in another domain.
    ADS_UF_NORMAL_ACCOUNT = 0x0200, // This is a default account type that represents a typical user.
    ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800, // This is a permit to trust account for a system domain that trusts other domains.
    ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000, // This is a computer account for a computer that is a member of this domain.
    ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000, // This is a computer account for a system backup domain controller that is a member of this domain.
    ADS_UF_DONT_EXPIRE_PASSWD = 0x10000, // The password should never expire on the account.
    ADS_UF_MNS_LOGON_ACCOUNT = 0x20000, // This is an MNS logon account.
    ADS_UF_SMARTCARD_REQUIRED = 0x40000, // The user must log on using a smart card.
    ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000, // The account is enabled for delegation.
    ADS_UF_NOT_DELEGATED = 0x100000, // The account is not enabled for delegation.
    ADS_UF_USE_DES_KEY_ONLY = 0x200000, // Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys.
    ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000, // This account does not require Kerberos pre-authentication for logon.
    ADS_UF_PASSWORD_EXPIRED = 0x800000, // The user's password has expired.
    ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000 // The account is trusted to authenticate a user outside of the Kerberos security package and delegate that user through constrained delegation.
}
}