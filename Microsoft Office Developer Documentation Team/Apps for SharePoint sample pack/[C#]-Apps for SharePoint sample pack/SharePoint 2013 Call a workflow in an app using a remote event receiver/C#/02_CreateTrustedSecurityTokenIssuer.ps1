Add-PSSnapin "Microsoft.SharePoint.PowerShell"

$issuerID = "11111111-1111-1111-1111-111111111111"

# TODO; specify domain name for SSL certificate
$targetSiteUrl = "http://<ENTER NAME OF TARGET SITE>"
# example
# $targetSiteUrl = "http://dev.wingtip.com"
$targetSite = Get-SPSite $targetSiteUrl
$realm = Get-SPAuthenticationRealm -ServiceContext $targetSite

$registeredIssuerName = $issuerID + '@' + $realm

# TODO; specify fully qualified path to the certificate
$publicCertificatePath = "C:\<PATH TO CERT>.cer"
# example
# $publicCertificatePath = "C:\MyCert.cer"
$publicCertificate = Get-PfxCertificate $publicCertificatePath

Write-Host "Create token issuer"
$secureTokenIssuer = New-SPTrustedSecurityTokenIssuer -Name $issuerID -RegisteredIssuerName $registeredIssuerName -Certificate $publicCertificate -IsTrustBroker

$secureTokenIssuer | select *
$secureTokenIssuer  | select * | Out-File -FilePath "SecureTokenIssuer.txt"


$serviceConfig = Get-SPSecurityTokenServiceConfig
$serviceConfig.AllowOAuthOverHttp = $true
$serviceConfig.Update()

Write-Host "All done..."