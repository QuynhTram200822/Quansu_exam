$(document).ready(function () {
  $(".news-section").each(function () {
    const $section = $(this);
    const $tabs = $section.find(".tab");
    const $tabWrapper = $section.find(".tab-cnt");

    function applyResponsiveLayout() {
      const width = $(window).width();

      if (width < 480) {
        setActiveTabByTarget("#grid");
      } else if (width < 768) {
        setActiveTabByTarget("#list");
      } else {
        const $activeTab = $tabs.filter(".active");
        const target = $activeTab.length ? $activeTab.data("tab-target") : "#grid";
        toggleLayoutByTarget(target);
      }
    }

    function setActiveTabByTarget(targetSelector) {
      $tabs.each(function () {
        const $tab = $(this);
        $tab.toggleClass("active", $tab.data("tab-target") === targetSelector);
      });
      toggleLayoutByTarget(targetSelector);
    }

    function toggleLayoutByTarget(target) {
      $tabWrapper.removeClass("list-view grid-view");
      const layoutClass = target.replace("#", "") + "-view"; 
      $tabWrapper.addClass(layoutClass);
    }

    $tabs.on("click", function () {
      const target = $(this).data("tab-target");
      setActiveTabByTarget(target);
    });

    $(window).on("resize", function () {
      setTimeout(applyResponsiveLayout, 100);
    });

    // Gọi lần đầu khi load cho từng section
    applyResponsiveLayout();
  });
});


