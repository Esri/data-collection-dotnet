# Release notes

## 1.3.0

* Updates both WPF and UWP with a new design that adapts to work well for any screen size.
    * Adds a `ModernMapPanel` custom layout panel to facilitate consistent, responsive design for UWP and WPF.
    * Refactors styles and related XAML.
    * Switches to vector icons where possible.
* Moves the .NET Core WPF project to a separate folder to prevent possible build issues.
* Updates the code called at sign out to revoke credentials from the server, when possible, and delete them locally.
* Adds support for identifying mulitple items on the map simultaneously and displaying them in a list.
    * An Arcade expression defined per layer is used to create subtitles that help you differentiate identified features.
* Adds unit tests for new code added to support multiple identify results.
    * Changes were made to shared code to facilitate unit testing of view models.
* Adds a new 'About' page, which shows the app version, ArcGIS Runtime version, and a link to a page detailing licenses. The appearance of these fields can be controlled via the app's configuration.
* Updates doc and many code comments to avoid all use of `http:`.
* Updates ArcGIS Runtime to 100.10.

## 1.2.3

* Updates all versions of the app to use the latest 100.9 release of ArcGIS Runtime.

## 1.2.2

* Adds doc table of contents to root README.md and docs/index.md
* Renames docs/index.md to [docs/README.md](/docs/README.md)

## 1.2.1

* Updates all versions of the app to use the latest 100.8.0 release of ArcGIS Runtime.
* Updates the UWP minimum target platform from 16299 to 17134

## 1.2.0

* Adds support for viewing and navigating to bookmarks.
* Adds support for toggling layer visibility with a TOC.
* Adds support for viewing the legend (layer symbols) for the map.
* Updates design for UWP and makes better use of acrylic.
* Removes the dependency on the Windows Community Toolkit from the UWP version of the app.
* Adds dependency on WinUI 2 to the UWP version of the app.
* Replaces most icons with beautiful new icons from the Calcite design system.
* Fixes a bug that prevented cancellation of map synchronization.
* Updates the design of the WPF app to more closely resemble the UWP app.

## 1.1.1

* Support for .NET Core.
* Resolves all compiler warnings.
* Fixes broken image link in README.

## 1.1.0

This is the third release of the Data Collection app for .NET, built on ArcGIS Runtime 100.7.

New features at 1.1.0 include:

* Upgraded to the latest 100.7 ArcGIS Runtime
* Support for viewing, adding, and deleting feature attachments
* UWP implementation of the app, built on the same shared foundation
* Bug fixes and performance improvements

## 1.0.1

New features at 1.0.1 include:

* Comprehensive [app documentation](/docs/README.md) from the ArcGIS for Developers site.

## 1.0.0

This is the first release of the Data Collection app for .NET. This app is designed to be cross-platform, with a shared core and a separate UI project supporting WPF.

At 1.0.0, Data Collection for .NET supports the following features:

* Authenticate with OAuth
* Configure the app using a web map from ArcGIS Online
* Identify features on the map and view attributes using popups
* Zoom to the starting point defined by the web map
* Collect new features
* Edit existing features using popups
* Create and edit related records, with support for one-to-many and many-to-one relationships
* Edit data offline with support for the [on-demand workflow](https://developers.arcgis.com/net/latest/wpf/guide/offline.htm#ESRI_SECTION1_AAADEDF10BF24FDF88DBF6EF04DF8579)
* Sync edits made offline with the server when online
* Delete or refresh the previously downloaded offline map
