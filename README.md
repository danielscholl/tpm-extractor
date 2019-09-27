# tpm-extractor

[![Build Status](https://dascholl.visualstudio.com/IoT/_apis/build/status/danielscholl.tpm-extractor?branchName=master)](https://dascholl.visualstudio.com/IoT/_build/latest?definitionId=51&branchName=master)

Retrieve a TPM 2.0 Enrollment Key to be used with the Azure Device Provisioning Service.

## Usage

Pass <RegistrationId> (optional) to override the `hostname` as the default RegistrationId
Pass <RegistrationId> and <IdScope> to execute DPS Provisioning.

Usage: [sudo] dotnet tpm.dll \<RegistrationId\> \<IdScope\> 

*Note*: Run this 'As Administrator' (Windows Powershell) or 'SUDO' (Linux)

## Execution

1. Run this application
2. Retrieve an Endorsement key for your device
3. Switch to the Azure Portal
4. In Azure Device Provisioning Service under 'Manage enrollments' select 'Individual Enrollments'
5. Select 'Add individual enrollment' and fill in:
    1. Mechanism 'TPM'
    2. Endorsement key
    3. Registration Id
6. Save the individual enrollment


```powershell
$RELEASE="20190927.1"
$ID_SCOPE="<your_idscope>"
$REGISTRATION="edge-gateway"

. {Invoke-WebRequest -useb https://github.com/danielscholl/tpm-extractor/releases/download/$RELEASE/windows.zip -Outfile tpm-extractor.zip}
Expand-Archive -Path ".\tpm-extractor.zip"
./tpm-extractor/tpm-extractor

. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; `
Install-SecurityDaemon -Dps -ContainerOs Linux -ScopeId $ID_SCOPE -RegistrationId $REGISTRATION_ID
```

## Exceptions

If you get this 'TbsCommandBlocked' execption during the execution of the application:

    Unhandled Exception: Microsoft.Azure.Devices.Provisioning.Client.ProvisioningTransportException: AMQP transport exception ---> Tpm2Lib.TpmException: Error {TbsCommandBlocked} was returned for command ActivateCredential.

Check if you are running it as Adminsitrator or 'SU'.

