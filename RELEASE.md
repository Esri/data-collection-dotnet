# Release notes

## 1.0.1

- Comprehensive [app documentation](/docs/index.md) from the ArcGIS for Developers site.

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
