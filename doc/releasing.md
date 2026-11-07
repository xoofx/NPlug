# Releasing packages

All three package publishers use NuGet Trusted Publishing with GitHub OIDC.
No stored NuGet API key is required. The NuGet policy creator must match the
configured `user` (`xoofx`), not merely the package owner.

Configure NuGet policies for repository `xoofx/NPlug` and these workflow filenames:

| Package | Workflow filename | Version source |
| --- | --- | --- |
| NPlug | `ci.yml` | Git release tag, with the VST SDK build-number suffix |
| NPlug.Proxy | `ci-publish_nplug_proxy.yml` | `Version` in `src/NPlug.Proxy/NPlug.Proxy.msbuildproj` |
| NPlug.Validator | `ci-publish_nplug_validator.yml` | `Version` in `src/NPlug.Validator/NPlug.Validator.csproj`, including the VST SDK suffix |

The main CI uses the shared dotnet-releaser composite action and the OIDC settings
in `src/dotnet-releaser.toml`. Version 0.24.1 includes the fix that obtains the
short-lived publishing key close to publication. The composite action's legacy
`NUGET_TOKEN` input is deliberately empty; it is not a secret-key fallback.
Normal builds and pull requests do not publish packages; release tags do.

The two manually dispatched publishers run only on `main`. They build and pack
using the project's own version, then use `NuGet/login` immediately before upload
to avoid obtaining a short-lived key before a potentially long build. These
workflows need only `contents: read` and `id-token: write`; the main release job
also needs `contents: write` to create GitHub releases.

## Publication order

1. Check which packages changed and which versions are already available on NuGet.
2. If Proxy changed, bump its local version and publish it first. Otherwise reuse
   the existing published version; do not republish an unchanged version.
3. If Validator changed, bump its local version, publish it and verify public
   NuGet availability. Confirm that its Proxy dependency is already available.
4. Only then push the NPlug release tag. For example, tag `0.7.0` with VST SDK
   `3.8.1` produces NPlug package `0.7.0.381`. Confirm its Proxy dependency is also
   already available.

Trusted-publishing policies must be ready before dispatching a publisher or
pushing a release tag. Workflow/configuration changes alone do not require
regenerating native binaries.
