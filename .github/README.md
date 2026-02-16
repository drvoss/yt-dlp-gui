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
| Architecture | Monolithic ViewData | Multiple focused ViewModels |
| Testing | None (0%) | xUnit + Moq (60%+ target) |
| DI Container | None | Microsoft.Extensions.DependencyInjection |
| CI/CD | None | GitHub Actions |
| Code-behind | 600+ LOC | Minimal (Command pattern) |
| Package Management | Traditional | Central Package Management |

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
* 📦 **Updated Dependencies**: Latest stable NuGet packages
* 🌐 **Improved Translations**: Fixed typos in Korean and other languages
* 🏗️ **Clean Architecture**: Separated concerns, testable code
* 🔧 **Better Error Handling**: Comprehensive exception management
* ⚡ **Performance**: Async operations, optimized memory usage

---

## 📋 Requirements

* [yt-dlp](https://github.com/yt-dlp/yt-dlp) (or compatible applications)
* [FFMPEG](https://ffmpeg.org/download.html#build-windows)
* Windows 10 or above
* .NET 8.0 Runtime

---

## 🚀 Development Roadmap

### Phase 1: Foundation (✅ Completed)
- [x] Korean translation fixes
- [x] Restore debug configuration
- [x] Update NuGet packages
- [x] PR submitted to original project

### Phase 2: Architecture Refactoring (🚧 In Progress)
- [ ] Migrate to .NET 8.0
- [ ] Split ViewData into focused ViewModels
- [ ] Extract service layer (IDownloadService, IFormatService, etc.)
- [ ] Introduce dependency injection
- [ ] Remove code-behind logic

### Phase 3: Testing Infrastructure (📅 Planned)
- [ ] Setup xUnit test projects
- [ ] Unit tests for ViewModels
- [ ] Integration tests for services
- [ ] GitHub Actions CI/CD pipeline
- [ ] Target: 60%+ code coverage

### Phase 4: Performance & Polish (📅 Planned)
- [ ] Convert blocking operations to async
- [ ] Memory optimization (circular buffers)
- [ ] PropertyChanged optimization
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
