# Changelog

## [1.2.0]

### Changed

- Bumped target framework from netstandard1.3 to netstandard2.0
- Only counts `id`, `auth_date` and `hash` fields as mandatory

## [1.1.0]

### Fixed

- Now validates hashes with `last_name` set as well

## [1.0.0]

### Added

- A 'WidgetEmbedCodeGenerator' class
- Tests for the 'LoginWidget.CheckAuthorization' method
- Login Widget with support for validating authorization hashes
