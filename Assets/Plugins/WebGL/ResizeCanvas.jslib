var ResizeCanvas = {
  Resize: function() {
    var ctx = (document.getElementById('canvas'));
    ctx.width  = window.innerWidth;
    ctx.height = window.innerHeight;
  }
};
mergeInto(LibraryManager.library, ResizeCanvas);