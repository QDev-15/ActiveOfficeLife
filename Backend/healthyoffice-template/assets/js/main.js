// Reading progress for article pages
(function(){
  const bar = document.getElementById('progress');
  const article = document.querySelector('article');
  if(!bar || !article) return;
  const onScroll = () => {
    const rect = article.getBoundingClientRect();
    const total = article.scrollHeight - window.innerHeight + rect.top;
    const scrolled = Math.min(Math.max(window.scrollY - (article.offsetTop - 70), 0), total);
    const pct = total>0 ? (scrolled/total)*100 : 0;
    bar.style.width = pct + '%';
  };
  window.addEventListener('scroll', onScroll, {passive:true});
  onScroll();
})();
