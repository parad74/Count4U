var DrawType = {
    'SHELF': 0,
    'WALL': 1,
    'WINDOW': 2,
};

var drawings = [];
var layer = null;

$(function () {

    var canvasWidth = 800;
    var canvasHeight = 800;

    var stage = new Kinetic.Stage({
        container: 'container',
        width: canvasWidth,
        height: canvasHeight,
    });

    layer = new Kinetic.Layer();

    stage.add(layer);

    var shapesCount = 5;

    for (var i = 0; i < shapesCount; i++) {
        var x = getRandomInt(0, canvasWidth - 50);
        var y = getRandomInt(0, canvasHeight - 50);

        console.log("x: " + x, " y: " + y);

        var width = getRandomInt(50, 300);
        var height = getRandomInt(50, 300);

        var draw = null;

        //        var type = getRandomDrawingEnum();
        var type = DrawType.SHELF;
        switch (type) {
            case DrawType.SHELF:
                draw = buildShelf(x, y, width, height, getRandomInt(-15, 15));
                break;
            case DrawType.WALL:
                draw = buildWall(x, y, width, height);
                break;
            case DrawType.WINDOW:
                draw = buildWindow(x, y, width, height);
                break;
            default:
        }

        addDrawingToCollection(draw, type, 0);
        layer.add(draw);
    }

    layer.draw();

    //window.setTimeout(update, 1000);
    window.setInterval(update, 300);
});

function update() {

    var dataToSend = [];

    for (var i = 0; i < drawings.length; i++) {
        var drawing = drawings[i];

        if (drawing.type == DrawType.SHELF) {
            dataToSend.push({ id: drawing.id, done: drawing.done });
        }
    }

    console.log(dataToSend);

    $.ajax({
        type: 'POST',
        url: "/Home/Data",
        data: JSON.stringify(dataToSend),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            console.log(data);

            for (var j = 0; j < data.length; j++) {
                var dataItem = data[j];
                var drawItem = $.grep(drawings, function (e, n) { return e.id == dataItem.id; })[0];

                var count = drawItem.item.getChildren().length;

                console.log('child of item');

                for (var k = 0; k < count; k++) {
                    var shape = drawItem.item.getChildren()[k];

                    if (shape.className == 'Text') {
                        shape.setText(dataItem.done + '%');                     
                    }
                    
                    if (shape.className == 'Rect') {
                        shape.setFillRGB(rgbFromDone(dataItem.done));
                    }
                    
                    drawItem.done = dataItem.done;
                }
            }

            layer.draw();
        }
    });
}

function getRandomDrawingEnum() {
    var type = getRandomInt(0, 2);
    switch (type) {
        case 0:
            return DrawType.SHELF;
        case 1:
            return DrawType.WALL;
        case 2:
            return DrawType.WINDOW;
        default:
    }
}

function codeGenerate(length) {
    var raw = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var rawLength = raw.length;


    var result = '';

    for (var i = 0; i < length; i++) {
        var ch = raw.charAt(getRandomInt(0, rawLength));
        result += ch;
    }

    return result;
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function buildWall(x, y, width, height) {
    var rect = new Kinetic.Rect({
        x: x,
        y: y,
        width: width,
        height: height,
        fill: 'lightgray',
        stroke: 'gray',
        strokeWidth: 1
    });

    return rect;
}

function buildWindow(x, y, width, height) {
    var rect = new Kinetic.Rect({
        x: x,
        y: y,
        width: width,
        height: height,
        fill: 'white',
        stroke: 'darkgray',
        strokeWidth: 2
    });

    return rect;
}

function buildShelf(x, y, width, height, angle) {
    var group = new Kinetic.Group(
       {
           x: x,
           y: y,
           rotationDeg: angle,
           //           offset: [width / 2, height / 2]
       });

    var rect = new Kinetic.Rect({
        width: width,
        height: height,
        //fill: 'green',
        fillRGB: rgbFromDone(0),
        strokeRGB: {
            r: 255,
            g: 140,
            b: 0,
        },
        strokeWidth: 2,
    });

    var text = new Kinetic.Text({
        x: 15,
        y: rect.getHeight() / 2 - 10,
        text: '0%',
        fontSize: 20,
        fontFamily: 'Calibri',
        fill: 'black',
    });

    group.add(rect);
    group.add(text);

    return group;
}

function addDrawingToCollection(drawing, type, done) {
    drawings.push({
        'id': codeGenerate(5),
        'type': type,
        'item': drawing,
        'done': done,
    });
}

function rgbFromDone(p) {
    if (p < 0 || p > 100) return null;

    var r1 = 160;
    var g1 = 160;
    var b1 = 160;
    var r2 = 38;
    var g2 = 127;
    var b2 = 0;

    var r3 = Math.round(r1 + (((r2 - r1) * p) / 100));
    var g3 = Math.round(g1 + (((g2 - g1) * p) / 100));
    var b3 = Math.round(b1 + (((b2 - b1) * p) / 100));

    return { r: r3, g: g3, b: b3 };
}