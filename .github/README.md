# yt-dlp-gui (Modern Fork)

[![Original Repository](https://img.shields.io/badge/original-kannagi0303%2Fyt--dlp--gui-blue)](https://github.com/kannagi0303/yt-dlp-gui)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/kannagi0303/yt-dlp-gui)](#)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![License](https://img.shields.io/github/license/kannagi0303/yt-dlp-gui)](#)

> **🍴 Fork Notice**: This is a modernized fork of [kannagi0303/yt-dlp-gui](https://github.com/kannagi0303/yt-dlp-gui) focused on architectural improvements, code quality, and maintainability while preserving all original features.

---

## 🎯 Fork Goals

This fork aims to:

- ✨ **Modernize Architecture**: Refactor to clean MVVM pattern with SOLID principles
- 🚀 **Upgrade Platform**: Migrate from .NET 6.0 to .NET 8.0 (LTS)
- 🧪 **Add Testing**: Introduce comprehensive unit and integration tests (target: 60%+ coverage)
- 📦 **Improve Maintainability**: Extract god objects, introduce dependency injection
- 🌍 **Enhance i18n**: Improve translation quality and add more languages
- ⚡ **Optimize Performance**: Implement async/await patterns, memory optimization

---

## 🔄 Differences from Original

| Aspect | Original | This Fork |
|--------|----------|-----------|
| .NET Version | 6.0 (EOL Nov 2024) | 8.0 (LTS until Nov 2026) |
| Architecture | Monolithic ViewData (86 props) | 11 focused ViewModels (21 props, 76% reduction) |
| Testing | None (0%) | xUnit + Moq (60%+ target, planned) |
| DI Container | None | Microsoft.Extensions.DependencyInjection (planned) |
| CI/CD | None | GitHub Actions ✅ |
| Code-behind | 600+ LOC | Refactoring in progress |
| Package Management | Traditional | Central Package Management ✅ |

---

## 📸 Screenshots
<img src="screenshot01.png" width="640"/>

---

## ✨ Features

### Core Features (from original)
* Easy-to-use GUI for [yt-dlp](https://github.com/yt-dlp/yt-dlp)
* Windows Only (10 or above, due to .NET 8.0 requirements)
* Portable application
* Selectable video quality
* Download single chapter/stream
* Cookie and configuration support
* External downloader support (Aria2)
* Multi-language support ([supported languages](/languages))

### Modern Fork Enhancements
* 🐛 **Better Debugging**: Full debug symbols for Visual Studio
* 📦 **Updated Dependencies**: Latest stable NuGet packages (.NET 8.0, Markdig 0.45.0, etc.)
* 🌐 **Improved Translations**: Fixed typos in Korean and other languages
* 🏗️ **Clean Architecture**: Multi-project solution with proper separation of concerns
* 🎯 **Modern Platform**: .NET 8.0 LTS with C# 12 features
* 📁 **Organized Structure**: 4-layer architecture (Core, Services, ViewModels, WPF)
* 📦 **Central Package Management**: Single source of truth for all dependencies
* 🔄 **CI/CD Pipeline**: Automated builds with GitHub Actions
* 🧩 **Focused ViewModels**: 11 single-responsibility ViewModels extracted from God Object (76% reduction!)
* 🔧 **Better Maintainability**: 58 files reorganized for easier testing and development

---

## 📋 Requirements

* [yt-dlp](https://github.com/yt-dlp/yt-dlp) (or compatible applications)
* [FFMPEG](https://ffmpeg.org/download.html#build-windows)
* Windows 10 or above
* .NET 8.0 Runtime

---

## 🚀 Development Roadmap

### Phase 1: Foundation (✅ Completed - 2026-02-16)
- [x] Korean translation fixes
- [x] Restore debug configuration
- [x] Update NuGet packages
- [x] PR submitted to original project (#175)

### Phase 2: Architecture Modernization (🚧 In Progress - Phase 2-2 Week 3)

#### Phase 2-1: Project Modernization (✅ Complete - 2026-02-16)
- [x] **Migrate to .NET 8.0** - Upgraded from .NET 6.0 to .NET 8.0 LTS with C# 12 support
- [x] **Multi-Project Solution** - Restructured into 4 projects (Core, Services, ViewModels, WPF)
  - YtDlpGui.Core: Domain models and interfaces
  - YtDlpGui.Services: Business logic and service implementations
  - YtDlpGui.ViewModels: MVVM layer (prepared)
  - YtDlpGui.WPF: UI layer with Views, Controls, Themes
- [x] **Central Package Management** - Directory.Packages.props for centralized version control
- [x] **GitHub Actions CI/CD** - Automated build pipeline on Windows runner

#### Phase 2-2: Architecture Refactoring (🚧 In Progress - 76% Complete)
- [x] **Extract ViewData into focused ViewModels** - 11 ViewModels created
  - ✅ UpdateViewModel (5 properties) - Update check functionality
  - ✅ WindowStateViewModel (8 properties) - Window position, size, scale
  - ✅ NetworkSettingsViewModel (2 properties) - Proxy configuration
  - ✅ DownloadProgressViewModel (11 properties) - Download status tracking
  - ✅ OutputPathViewModel (5 properties) - File path management
  - ✅ FormatSelectionViewModel (6 properties) - Video/audio format selection
  - ✅ DownloadOptionsViewModel (10 properties) - Download settings
  - ✅ CookieSettingsViewModel (3 properties) - Cookie management
  - ✅ PathsViewModel (5 properties) - External tool paths
  - ✅ UIStateViewModel (10 properties) - UI state flags and display properties
  - **Result**: ViewData reduced from 86 → 21 properties (**76% reduction!** 🎉)
- [ ] Create service interfaces (IDownloadService, IFormatService, etc.)
- [ ] Introduce dependency injection container
- [ ] Remove code-behind logic with Command pattern

### Phase 3: Testing Infrastructure (📅 Planned)
- [ ] Setup xUnit test projects for Core and Services
- [ ] Unit tests for ViewModels
- [ ] Integration tests for Services
- [ ] Automated test execution in CI/CD
- [ ] Target: 60%+ code coverage

### Phase 4: Performance & Polish (📅 Planned)
- [ ] Convert blocking operations to async/await
- [ ] Memory optimization (circular buffers for logs)
- [ ] PropertyChanged optimization with value comparison
- [ ] Startup time improvement
- [ ] UI responsiveness enhancements

---

## 🤝 Contributing

### For Original Project
Small bug fixes, translation improvements, and minor enhancements should be contributed to the **[original repository](https://github.com/kannagi0303/yt-dlp-gui)**.

### For This Fork
Contributions welcome for:
- 🏗️ Architectural improvements
- 🧪 Test coverage
- ⚡ Performance optimizations
- 🌍 Translation enhancements
- 📚 Documentation updates

**Development Guidelines**:
1. Fork this repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Ensure tests pass and coverage is maintained
4. Commit with conventional commits (`feat:`, `fix:`, `refactor:`, etc.)
5. Push and create a Pull Request

---

## 📚 Documentation

* Original project wiki: [GitHub Wiki](https://github.com/kannagi0303/yt-dlp-gui/wiki)
* Architecture documentation: [ARCHITECTURE.md](ARCHITECTURE.md) *(coming soon)*
* Contributing guide: [CONTRIBUTING.md](CONTRIBUTING.md) *(coming soon)*

---

Please refer to the [original wiki](https://github.com/kannagi0303/yt-dlp-gui/wiki) for usage and details.

* Front-end of [yt-dlp](https://github.com/yt-dlp/yt-dlp) (and Compatible Applications)

## 🙏 Credits

### Original Author
* **かんなぎ (Kannagi)** - [kannagi0303](https://github.com/kannagi0303)
  * Original project: [kannagi0303/yt-dlp-gui](https://github.com/kannagi0303/yt-dlp-gui)

### Fork Maintainer
* **drvoss** - This modernized fork

### Third Party
* [yt-dlp](https://github.com/yt-dlp/yt-dlp) - The excellent command-line tool
* [youtube-dlp-gui-installer](https://github.com/kazukikasama/youtube-dlp-gui-installer) - Automatic installer by [kazukikasama](https://github.com/kazukikasama)

---

## 📞 Contact

* For **original project** questions: Use [original repo's Issues/Discussions](https://github.com/kannagi0303/yt-dlp-gui/issues)
* For **fork-specific** questions: Use [this fork's Issues](../../issues)

Languages supported:
* English ✅
* 한국어 (Korean) ✅
* 中文 (Chinese) ✅
* 日本語 (Japanese) ✅

---

## 📄 License

This project inherits the license from the original [kannagi0303/yt-dlp-gui](https://github.com/kannagi0303/yt-dlp-gui) repository.

---

## ⭐ Star History

If you find this fork useful, please consider:
- ⭐ Starring the **[original repository](https://github.com/kannagi0303/yt-dlp-gui)**
- ⭐ Starring this fork
- 🐛 Reporting issues
- 🤝 Contributing improvements
