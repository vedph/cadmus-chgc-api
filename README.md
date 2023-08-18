# Cadmus CHGC API

Quick Docker image build:

    docker build . -t vedph2020/cadmus-chgc-api:3.0.5 -t vedph2020/cadmus-chgc-api:latest

(replace with the current version).

This is a Cadmus API layer customized for the CHGC project (*Cadmus Compendium Historiae in genealogia Christi*).

- [core](https://github.com/vedph/cadmus-chgc)
- [API](https://github.com/vedph/cadmus-chgc-api)
- [app](https://github.com/vedph/cadmus-chgc-app)

## History

### 3.0.5

- 2023-08-18: updated packages for refactored XML output.

### 3.0.4

- 2023-08-09: added import thesauri endpoint to import controller.

### 3.0.3

- 2023-08-06: updated packages.
- 2023-08-01: updated packages.

### 3.0.2

- 2023-07-28: updated packages after TEI export refactoring.
- 2023-07-27: updated packages.
- 2023-07-20: refactored [logging](https://myrmex.github.io/overview/cadmus/dev/history/b-logging).

### 3.0.1

- 2023-07-18:
  - updated packages (models and importer).
  - added import controller.

### 3.0.0

- 2023-07-16: refactored image annotations system.
- 2023-07-12: updated packages.
- 2023-07-10: updated packages.

### 2.0.2

- 2023-07-07: updated packages.
- 2023-07-06: updated packages.

### 2.0.1

- 2023-07-01: added export API.
- 2023-06-27: only 5 seeded items in config.
- 2023-06-24: updated profile.
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
