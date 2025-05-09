# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-05-09
### Added
- Initial release of URT3D SDK
- Core functionality for loading and managing URT3D assets
- Asset system with Actor, Preview, and Metadata components
- Wrapper MonoBehaviour for easy integration in Unity scenes
- Support for both local file and remote GUID-based asset loading
- Scriptable traits system for extending asset functionality
- MiniScript scripting engine for asset behavior programming
- Script triggers: OnLoad, OnUpdate, and OnCustomEvent
- Configurable script execution modes: EditorOnly, RuntimeOnly, and Both
- Editor tools for asset inspection and configuration
- Sample scenes demonstrating SDK usage
- Sample content: encrypted and unencrypted asset examples
- Documentation structure for SDK reference

### Dependencies
- Unity Cloud GLTFast 6.12.1
- Unity Editor Coroutines 1.0.0
- Unity Input System 1.13.1
- Unity Newtonsoft JSON 3.2.1
- Unity Sharp Zip Lib 1.3.9
- Unity UGUI 2.0.0
