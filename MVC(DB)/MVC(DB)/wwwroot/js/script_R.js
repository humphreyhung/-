// 1. 登入按鈕點擊行為
document.querySelector('.login-btn').addEventListener('click', () => {
    alert("#");
});

// 2. 導覽列按鈕模擬
document.querySelectorAll('nav button').forEach(btn => {
    btn.addEventListener('click', () => {
        alert(`你點擊了「${btn.textContent}」`);
    });
});

// 3. 搜尋功能
document.querySelector('nav input[type="text"]').addEventListener('input', (e) => {
    const keyword = e.target.value.trim().toLowerCase();
    const projects = document.querySelectorAll('.project');

    projects.forEach(project => {
        const title = project.querySelector('h2').textContent.toLowerCase();
        project.parentElement.style.display = title.includes(keyword) ? 'block' : 'none';
    });
});

// 4. 顯示 progress 條百分比
document.querySelectorAll('.project').forEach(project => {
    const progressBar = project.querySelector('progress');
    const value = parseInt(progressBar.value);
    const max = parseInt(progressBar.max);

    const percentageText = document.createElement('p');
    percentageText.style.fontWeight = 'bold';
    percentageText.textContent = `達成率：${Math.round((value / max) * 100)}%`;
    project.insertBefore(percentageText, progressBar.nextSibling);
});

// 5. 瀏覽人次模擬自動增加
let viewCountSpan = document.querySelector('.see');
let baseNumber = parseInt(viewCountSpan.textContent.replace(/[^\d]/g, ''));
setInterval(() => {
    baseNumber += Math.floor(Math.random() * 3); // 每秒加 0~2
    viewCountSpan.textCont})
