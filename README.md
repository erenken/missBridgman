# Miss Bridgman Scholarship Organization

The replacement site is a .NET 11 Blazor Web App. It serves the visitor pages and JSON API from one ASP.NET Core host. The previous Angular app (`mb-app/`) and Azure Functions API (`api/`) remain in the repository as migration references; the current app is in `MissBridgman.Web/`.

## Run locally

Use the .NET 11 SDK and open `MissBridgman.slnx` in Visual Studio. Set `MissBridgman.AppHost` as the startup project to run the site under Aspire. From the repository root, keep local credentials in the AppHost user-secrets store:

```powershell
dotnet user-secrets set "Admin:Password" "use-a-long-unique-password" --project .\MissBridgman.AppHost\MissBridgman.AppHost.csproj
dotnet user-secrets set "AzureWebJobsStorage" "<storage-connection-string>" --project .\MissBridgman.AppHost\MissBridgman.AppHost.csproj
dotnet user-secrets set "APPLICATIONINSIGHTS_CONNECTION_STRING" "<Application Insights connection string>" --project .\MissBridgman.AppHost\MissBridgman.AppHost.csproj
dotnet run --project .\MissBridgman.AppHost\MissBridgman.AppHost.csproj
```

The AppHost forwards these settings to the Blazor app. To run just the web project, select `MissBridgman.Web` as the startup project instead.

The API uses the existing `images` Blob container. The container must exist and allow public blob reads because gallery images are served with their blob URLs. Uploads accept JPG, PNG, and WebP images up to 12 MB. Each image stores its name, title, year, description, and category in blob metadata. Images without metadata from the previous site remain visible by filename.

Without a storage connection string, the site and forms still run and the gallery shows an empty state; photo uploads cannot be saved.

## Pages and API

- `/` - organization home and recent photos
- `/about` - program overview
- `/gallery` - photo gallery with a year filter
- `/forms` - built-in participation interest form with print / save-to-PDF and email handoff
- `/admin` - administrator sign-in and photo uploads
- `GET /api/photos?year=2026` - public gallery data
- `POST /api/admin/login` and `POST /api/admin/logout` - administrator session
- `POST /api/admin/photos` - authenticated multipart image upload and metadata
- `/healthz` - App Service health probe

Form entries remain in the active form session and aren't saved to application storage. The form page's **Save form as PDF** button opens the browser print dialog; choose **Save as PDF** to download a copy with the completed fields and drawn contestant/guardian signatures. The page links to the committee contact email so the visitor can attach the downloaded PDF. The site does not store or email form submissions.

## Deploy to Azure App Service

`infra/main.bicep` provisions a Linux App Service Plan (B1 by default), a built-in .NET 11 Linux Web App, a GitHub OIDC identity, DNS records for the apex and `www` hostnames, and SNI hostname bindings using the existing App Service managed certificates. GitHub Actions publishes a self-contained `linux-x64` package and deploys it as a ZIP directly to App Service. The package includes the .NET runtime and an explicit startup script, so the app does not depend on the exact .NET runtime patch installed in the App Service image. The workflow sets application secrets and the existing `missBridgmanApi` Application Insights connection string, then checks app health. No Docker image or Azure Container Registry is used. The workflow leaves the old Static Web App and Function App in place.

The ASP.NET Core API uses Microsoft's OpenTelemetry distribution to export server requests, dependencies, logs, and metrics to `missBridgmanApi`. Browser page views, JavaScript errors, and fetch dependencies use the supported Application Insights JavaScript SDK and carry W3C trace context for correlation with server telemetry. Browser monitoring uses the public Application Insights connection string and can be disabled locally by leaving `APPLICATIONINSIGHTS_CONNECTION_STRING` unset.

The person deploying the infrastructure must be signed into Azure CLI and have permission to create the resources, update the existing DNS zone, and create role assignments (typically Owner, or Contributor plus User Access Administrator / Role Based Access Control Administrator). The GitHub deployment identity receives Website Contributor on the Web App; the workflow no longer needs DNS permissions. The existing App Service managed certificates named `missbridgman.com` and `www.missbridgman.com` must be present before applying this template; Bicep attaches them to the hostname bindings. If a new hostname or certificate is needed later, provision its managed certificate first, then update and apply the Bicep template. Routine GitHub deployments do not modify DNS or TLS bindings.

Earlier infrastructure deployments may have already assigned DNS Zone Contributor to the GitHub identity. Because this template uses incremental deployment, removing that assignment from Bicep does not revoke an existing assignment; remove the old DNS Zone Contributor grant from the identity after applying the updated template.

From PowerShell, create the resources and the branch-scoped GitHub identity:

```powershell
./infra/deploy.ps1
```

The script defaults to the existing `missBridgman` resource group (located in North Central US), deploys the Web App resources in Central US, and targets repository `erenken/missBridgman` with Web App name `missbridgman-web-2026`. Change `-WebAppName` if that global App Service name is unavailable.

The Bicep command outputs the Web App name and GitHub OIDC identifiers. Copy those outputs into GitHub Settings > Secrets and variables > Actions:

### Repository secrets

| Secret | Value |
| --- | --- |
| `AZURE_CLIENT_ID` | `githubClientId` output from the Bicep deployment |
| `AZURE_TENANT_ID` | `githubTenantId` output |
| `AZURE_SUBSCRIPTION_ID` | `githubSubscriptionId` output |
| `ADMIN_PASSWORD` | A long, unique password for `/admin` |
| `AZURE_STORAGE_CONNECTION_STRING` | Connection string for the existing Storage account that contains the `images` container |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Connection string for the existing `missBridgmanApi` Application Insights component |

### Repository variables

| Variable | Value |
| --- | --- |
| `AZURE_RESOURCE_GROUP` | Resource group passed to `deploy.ps1` |
| `AZURE_WEBAPP_NAME` | `webAppName` output from Bicep |

The workflow reads the connection string and admin password at deploy time and sets the App Service application settings `AzureWebJobsStorage` and `Admin__Password`. These values are not passed to Bicep or committed to the repository. After adding the GitHub values, run the workflow from Actions > Build and deploy Miss Bridgman, or push a change to `MissBridgman.Web/`, `infra/`, or the deployment workflow to `main`. Apply `infra/main.bicep` with `./infra/deploy.ps1` whenever infrastructure, DNS, or TLS bindings need to change. Its apex A record and `www` CNAME point to App Service, replacing any existing targets for those record sets.

## Migrate existing image metadata

The existing `images` container has eight photos in year folders (2019 and 2023) and no blob metadata. Run `./infra/migrate-image-metadata.ps1` from an Azure CLI session with permission to read the account keys and update blob metadata. The migration carries over the 2019 court member names and titles from the current site, derives the 2023 names and titles from their existing filenames, sets the year from each folder, and leaves descriptions blank because the current site stores no descriptions. Categories for the 2019 photos use their current titles; the 2023 photos use `Court`. Metadata values are UTF-8/base64 encoded to match the new app's storage reader. The migration changes metadata only; it does not move or overwrite the image content.

The app requires WebSockets and Always On for Blazor Interactive Server, both enabled by this template. The B1 App Service Plan incurs ongoing Azure charges; choose a larger plan SKU through `-AppServiceSku` if the site's load requires it. Regional availability and pricing vary. HTTPS is enforced at App Service; forwarded headers are enabled for its TLS-terminating proxy.

This deployment does not delete the existing Static Web App or Function App or migrate the existing documents container. The Bicep deployment manages the apex and `www` DNS record sets, so review their current values before applying if they contain targets that should be retained. Keep the old resources until the new site is serving the existing photos correctly.
