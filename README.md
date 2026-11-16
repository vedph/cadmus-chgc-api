# Cadmus CHGC API

🐋 Before creating Docker images, ensure you have a buildx builder instance running that supports multi-arch and that you have run `buildnpub.ps1` publishing NuGet packages:

```sh
docker buildx create --use --name multi-arch-builder || docker buildx use multi-arch-builder
docker buildx inspect --bootstrap
```

Also, be sure to have run `buildnpub.ps1` for both local and remote NuGet feeds, so that the .NET CLI can restore all dependencies.

> To run natively on Linux VMs, macOS (both Intel and Apple Silicon), and Windows (via WSL2 or Docker Desktop)—`linux/amd64` and `linux/arm64` are the only two targets we need. Note that `docker buildx` automatically injects variables like `TARGETARCH` and `TARGETOS` into the scope of your build. In `Dockerfile` we pass these directly to the .NET CLI commands.

These commands build for multiple platforms and push directly to Docker Hub:

```sh
docker buildx build --platform linux/amd64,linux/arm64 -t vedph2020/cadmus-chgc-api:6.0.0 -t vedph2020/cadmus-chgc-api:latest --push .
```

(replace with the current version).

This is a Cadmus API layer customized for the CHGC project (*Cadmus Compendium Historiae in genealogia Christi*).

- [core](https://github.com/vedph/cadmus-chgc)
- [API](https://github.com/vedph/cadmus-chgc-api)
- [app](https://github.com/vedph/cadmus-chgc-app)
