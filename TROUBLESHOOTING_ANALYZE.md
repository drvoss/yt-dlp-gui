# yt-dlp-GUI Analyze 버튼 문제 진단 가이드

## 🔍 문제 진단 체크리스트

### 1. URL 입력 확인
- [ ] URL 입력란에 YouTube URL이 입력되어 있나요?
- [ ] URL이 완전한 형식인가요? (예: https://www.youtube.com/watch?v=...)
- [ ] URL 입력 후 Enter를 눌렀거나 포커스를 이동했나요?

**✅ 해결 방법**: 
- URL을 입력하고 Tab 키를 눌러 포커스를 이동시켜 보세요
- URL 입력란을 클릭한 후 다시 Analyze 버튼을 눌러보세요

### 2. Analyze 버튼 상태 확인
- [ ] Analyze 버튼이 회색(비활성화)으로 표시되나요?
- [ ] Analyze 버튼이 정상(활성화)으로 표시되나요?

**만약 버튼이 비활성화되어 있다면**:
- URL이 비어있거나 인식되지 않은 것입니다
- URL 입력란을 다시 확인하세요

### 3. 외부 도구 확인 (중요!)
Analyze 기능이 작동하려면 다음 파일이 필요합니다:

#### 필수 파일:
- `yt-dlp.exe` - YouTube 비디오 다운로더
- `ffmpeg.exe` - 비디오/오디오 변환 도구
- `aria2c.exe` - 다운로드 가속기 (선택사항)

#### 파일 위치:
1. **애플리케이션과 같은 폴더**
   `C:\work2\yt-dlp-GUI\src\YtDlpGui.WPF\bin\Debug\net8.0-windows10.0.17763.0\`

2. **또는 Preferences 메뉴에서 경로 설정**

### 4. 외부 도구 설치 방법

#### yt-dlp.exe 다운로드:
```powershell
# PowerShell에서 실행
$url = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe"
$output = "C:\work2\yt-dlp-GUI\src\YtDlpGui.WPF\bin\Debug\net8.0-windows10.0.17763.0\yt-dlp.exe"
Invoke-WebRequest -Uri $url -OutFile $output
Write-Host "✅ yt-dlp.exe 다운로드 완료"
```

#### ffmpeg.exe 다운로드:
1. https://ffmpeg.org/download.html#build-windows 방문
2. Windows 빌드 다운로드
3. ffmpeg.exe를 애플리케이션 폴더로 복사

#### aria2c.exe 다운로드:
```powershell
# PowerShell에서 실행 (선택사항)
$url = "https://github.com/aria2/aria2/releases/latest/download/aria2-*-win-64bit-build1.zip"
# 다운로드 및 압축 해제 후 aria2c.exe를 애플리케이션 폴더로 복사
```

### 5. 자동 설치 스크립트

아래 PowerShell 스크립트를 실행하여 자동으로 다운로드:

```powershell
$binPath = "C:\work2\yt-dlp-GUI\src\YtDlpGui.WPF\bin\Debug\net8.0-windows10.0.17763.0"
cd $binPath

# yt-dlp 다운로드
Write-Host "Downloading yt-dlp.exe..."
Invoke-WebRequest -Uri "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe" -OutFile "yt-dlp.exe"

Write-Host "✅ yt-dlp.exe downloaded"
Write-Host "⚠️ ffmpeg.exe는 수동으로 다운로드해야 합니다:"
Write-Host "   https://ffmpeg.org/download.html"
```

### 6. 디버그 로그 확인

Debug 빌드를 실행한 경우 Visual Studio Output 창에서 로그 확인:
```
=== Analyze Button Clicked ===
URL: <입력한 URL>
PathYTDLP: <yt-dlp.exe 경로>
DLP.Path_DLP: <yt-dlp.exe 경로>
Analyze_Start() called
Starting GetInfo()...
```

만약 "PathYTDLP: " 또는 "DLP.Path_DLP: "가 비어있다면 yt-dlp.exe가 없는 것입니다.

### 7. 일반적인 오류 메시지

| 증상 | 원인 | 해결방법 |
|------|------|----------|
| 버튼이 회색 | URL 미입력 | URL 입력 후 Tab 키 |
| 버튼을 눌러도 반응 없음 | yt-dlp.exe 없음 | yt-dlp.exe 다운로드 |
| "Sign in to confirm you're not a bot" | 쿠키 필요 | 쿠키 설정 활성화 |
| "This video is unavailable" | 영상 삭제/비공개 | 다른 영상 URL 시도 |

## 🎯 빠른 테스트

1. **URL 테스트**: https://www.youtube.com/watch?v=jNQXAC9IVRw
   - "Me at the zoo" - 첫 YouTube 영상
   
2. **단축 URL 테스트**: https://youtu.be/jNQXAC9IVRw

3. **재생목록 테스트**: https://www.youtube.com/playlist?list=...

## 📞 추가 도움

위 단계를 모두 확인했는데도 문제가 해결되지 않으면:
1. 정확한 에러 메시지 복사
2. URL 입력란의 내용 확인
3. yt-dlp.exe 파일 존재 여부 확인
4. 문제 재현 단계 상세 기록

---

**참고**: 이 문제는 리팩토링과 무관하며, 원래 애플리케이션의 정상적인 동작입니다.
yt-dlp GUI는 백엔드로 yt-dlp.exe를 사용하므로 해당 파일이 필수입니다.
