# VpnHelper

Reconnect to AnyConnect VPN with Windows stored credentials via GUI or CLI.

## CLI Output

```
Description:
  Connect to VPN if needed

Usage:
  VpnHelperCli [options]

Options:
  --server <server>                                Server to connect to.
  --saved-credential-name <saved-credential-name>  Name of saved Windows credential to use.  Set using cmdkey.exe or in Windows Credential Manager under Windows Credentials > Generic Credentials.
  --console-session-required                       Will switch to console session before connecting if true (required in some VPN configurations).
  --version                                        Show version information
  -?, -h, --help                                   Show help and usage information
  ```
