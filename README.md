# tpm-extractor

[![Build Status](https://dascholl.visualstudio.com/IoT/_apis/build/status/danielscholl.tpm-extractor?branchName=master)](https://dascholl.visualstudio.com/IoT/_build/latest?definitionId=51&branchName=master)

Retrieve a TPM 2.0 Enrollment Key to be used with the Azure Device Provisioning Service.

## Usage

Pass <RegistrationId> (optional) to override the `hostname` as the default RegistrationId
Pass <RegistrationId> and <IdScope> to execute DPS Provisioning.

Usage: [sudo] dotnet tpm.dll \<RegistrationId\> \<IdScope\> 

*Note*: Run this 'As Administrator' (Windows Powershell) or 'SUDO' (Linux)

## Install IoT Edge Runtime and Provision using TPM Flow

1. Download the IoT Edge Runtime
2. Run TPM Extractor
3. Retrieve the Endorsement Key for your device
4. Switch to the Azure Portal
5. In Azure Device Provisioning Service under 'Manage enrollments' select 'Individual Enrollments'
6. Select 'Add individual enrollment' and fill in:
    1. Mechanism 'TPM'
    2. Endorsement key
    3. Registration Id
7. Save the individual enrollment
8. Install the Security Daemon


```powershell
#  Download IoT Edge Runtime (Reboot Required)
. {Invoke-WebRequest -useb https://aka.ms/iotedge-win} | Invoke-Expression; Deploy-IoTEdge

# Download TPM Extractor
$RELEASE="20190927.5"
. {Invoke-WebRequest -useb https://github.com/danielscholl/tpm-extractor/releases/download/$RELEASE/windows.zip -Outfile tpm-extractor.zip}
Expand-Archive -Path ".\tpm-extractor.zip"

# Execute TPM Extractor
./tpm-extractor/tpm-extractor

# Copy & Paste Variables from Output
# -----------------------------------------------------------------------------------------------------
$EndorsementKey="<your_ekpublic>"
$RegistrationId="<your_registrationid>"
# -----------------------------------------------------------------------------------------------------


# Install Edge Runtime using DPS with the TPM
$ID_SCOPE="<your_idscope>"
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; `
Install-SecurityDaemon -Dps -ContainerOs Windows -ScopeId $ID_SCOPE -RegistrationId $RegistrationId


# Setup Docker Host Enviornment
[System.Environment]::SetEnvironmentVariable("DOCKER_HOST", "npipe:////./pipe/iotedge_moby_engine", [System.EnvironmentVariableTarget]::Machine)


# Configure
```

## Exceptions

If you get this 'TbsCommandBlocked' execption during the execution of the application:

    Unhandled Exception: Microsoft.Azure.Devices.Provisioning.Client.ProvisioningTransportException: AMQP transport exception ---> Tpm2Lib.TpmException: Error {TbsCommandBlocked} was returned for command ActivateCredential.

Check if you are running it as Adminsitrator or 'SU'.

