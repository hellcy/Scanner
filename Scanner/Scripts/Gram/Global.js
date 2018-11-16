function isPosInteger(x) {

    var flag = isNaN(x);
    
    if (flag === true) {
        flag = false;
    }
    else {
        flag = false;
        if ((x.indexOf(".") < 0) && (parseInt(x) > 0)) {
            flag = true;
        }       
    }
    return flag;
}

