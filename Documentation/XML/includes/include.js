<script type="text/javascript">
$(function(){
    var OnClick = function() {
        showContent($($(this).attr("href").replace(".", "\\.")));
    };
    
    var showContent = function(element){
        $("#CONTENT").html(element.html());
        $("#CONTENT > a").click(OnClick);
        $("#CONTENT").show();
        window.scrollTo(0, 0);
    };
    

    $("body > div:not(#TOC):not(#HEADER):not(#FOOTER):not(.vecto2):not(.vecto3)").hide();
    $("body > div:not(#TOC):not(#HEADER):not(#FOOTER) > div:not(.vecto2):not(.vecto3)").hide();

    window.onhashchange=function(){showContent($(window.location.hash.replace(".", "\\.")));};
    if (window.location.hash) {
        showContent($(window.location.hash.replace(".", "\\.")));
    } else {
        showContent($("#user-manual"));
    }
    
    $("#TOC").resizable();

    $("#TOC").resizable({
        handles: "e",
        resize: function(event, ui) {
            $("body > div:not(#TOC):not(#HEADER):not(#FOOTER)").css("padding-left", ui.size.width);
        }
    });
    
    $("#TOC").scroll(function() {
        $(".ui-resizable-handle").css('top', $("#TOC").scrollTop());
    });


/* hide some items from TOC */

$("#TOC li a[href='#electrical-auxiliaries-editor']").parent().hide()
$("#TOC li a[href='#combined-alternator-map-file-.aalt']").parent().hide()
$("#TOC li a[href='#pneumatic-auxiliaries-editor']").parent().hide()
$("#TOC li a[href='#hvac-auxiliaries-editor']").parent().hide()
/*-------------------------*/

    $("td[align=left").filter(function() {return $(this).text().indexOf("Locked default")===0 || $(this).text().indexOf("Locked Calc") === 0; }).addClass("aaux_locked")
});

/*
javascript to check if all links are valid:
*/

function CheckGeneratedLinks() {
	var content = jQuery("#CONTENT");
	content.html("");
	jQuery("a[href]").each(function(idx, elem) { 
		var link=jQuery(elem).attr("href"); 
		if(link.startsWith("javascript:")) {return true} 
		if (!link.startsWith("#")) { content.append("ignoring: " + link +"<br/>"); return true; } 
		var node=jQuery("*[id='" + link.replace("#", "") + "']"); 
		if (node.length) {
			content.append("found: " + link)
		} else {
			content.append("missing: " + link) 
		}; 
		content.append("<br/>"); 
	});
}

</script>