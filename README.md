# Cadmus CHGC API

Quick Docker image build:

    docker build . -t vedph2020/cadmus-chgc-api:2.0.0 -t vedph2020/cadmus-chgc-api:latest

(replace with the current version).

This is a Cadmus API layer customized for the CHGC project (*Cadmus Compendium Historiae in genealogia Christi*).

- [core](https://github.com/vedph/cadmus-chgc)
- [API](https://github.com/vedph/cadmus-chgc-api)
- [app](https://github.com/vedph/cadmus-chgc-app)

## History

- 2023-06-23: updated packages.

### 2.0.0

- 2023-06-17:
  - updated packages (refactored models).
  - moved to PostgreSQL.
- 2023-06-02: updated packages.

### 1.0.0

- 2023-05-24: updated packages (breaking change in general parts introducing [AssertedCompositeId](https://github.com/vedph/cadmus-bricks-shell/blob/master/projects/myrmidon/cadmus-refs-asserted-ids/README.md#asserted-composite-id)).
- 2023-05-16: updated packages.

### 0.0.2

- 2023-04-14: refactored to use backend CHGC components and added thesauri.

### 0.0.1

- initial release.
