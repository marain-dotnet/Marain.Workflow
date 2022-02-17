// <copyright file="GlobalSuppressions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

// Ideally we'd make this more targetted, but SupressMessage doesn't support patterns of any kind,
// and we don't really want to annotate every single place this crops up. (Perhaps
// Corvus.ContentHandling could define an optional IContentWithType defining the ContentType
// property that types can choose to implement, at which point we'd stop getting this message.)
[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "ContentType is read through reflection",
    Scope = "namespaceanddescendants",
    Target = "~N:Marain.Workflows")]