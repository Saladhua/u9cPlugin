const entityFun = require('../../../../Base/server/controllers/EntityFunction');
const entityCollection = require('../../../../Base/server/controllers/EntityCollection');
exports.reportQuery = async function (req, res) {
    let {body: param, Context} = req;
    try {
        const qcTasks = await getAggregateQcTask(req.body,Context.Org.Code);
        const retQcTasks = [];
        let curr = new Date().getTime();
        if (Array.isArray(qcTasks) && qcTasks.length) {
            for (let item of qcTasks) {

                item.defectDesc = item.PrivateDescSeg15;
                item.buyer = item.PrivateDescSeg16;
                item.project = item.PrivateDescSeg17;
                item.assembly = item.PrivateDescSeg18;
                item.assembly_detail = item.PrivateDescSeg19;
                item.latest_version = item.PrivateDescSeg20;
                let rec = item.RcvPlanRecord

                let timestamp = new Date(item.CompleteTime).valueOf() -  new Date(item.ArrivedTime).valueOf();
                let day = parseInt(timestamp / 86400000);
                let hour = parseInt(timestamp % 86400000 / 3600000);
                let minute = parseInt(timestamp % 3600000 / 60000);
                let second = parseInt(timestamp % 60000 / 1000);
                item.Durationtime =
                    (day ? day + '天' : '') +
                    (hour ? hour + '小时' : '') +
                    (minute ? minute + '分钟' : '') +
                    (second ? second + '秒' : '') ;
                if(Context.Org.Code == item.Org.Code){
                    retQcTasks.push(item);
                }

            }
        }
        console.log('ss=' + (new Date().getTime() - curr));
        res.json({
            Data: retQcTasks,
            Error: null
        });
    } catch (error) {
        res.json({
            Data: null,
            Error: error
        });
    }
};

function getAggregateQcTask(param,Org) {
    let args = [];
    const populate = [
        {path: 'ItemMaster', select: 'Code Name Specification Org'},
        {path: 'WorkLocation', select: 'Code Name'},
        {path: 'Work', select: 'Code Name'},
        {path: 'Supplier', select: 'Code Name'},
        {path: 'UOM', select: 'Code Name'},
        {path: 'WareHouse', select: 'Code Name'},
        {path: 'Bin', select: 'Code Name'},
        {path: 'Org', select: 'Code Name', model: 'Org'},
        {path: 'MasterOrg', select: 'Code Name', model: 'Org'},
        {path: 'DetectedOrg', select: 'Code Name', model: 'Org'},
        {path: 'Team', select: 'Code Name', model: 'Department'},
        {path: 'QCScheme', select: 'Code Name'},
        {path: 'Project', select: 'Code Name'},
        {path: 'ProductionLine', select: 'Code Name'},
        {path: 'InspectedBy', select: 'Code Name', model: 'User'},
        {path: 'SampedBy', select: 'Code Name', model: 'User'},
        {path: 'JudgedBy', select: 'Code Name', model: 'User'},
        {path: 'JobSchedule', select: 'Code Name'},
        //{path: 'RcvPlanRecord', select: '_id', model: 'LotNo'}
    ];
    populate.forEach(item => {
        const arr = item.select.split(' ');
        const $project = {};
        arr.forEach(key => {
            $project[key.trim()] = 1;
        });
        args.push({
            $lookup:
                {
                    from: item.model || item.path,
                    let: {
                        'lid': '$' + item.path
                    },
                    pipeline: [
                        {
                            $match: {
                                $expr: {$eq: ['$_id', '$$lid']}

                            }
                        },

                        {$project}
                    ],
                    as: item.path
                }

        });
        args.push({
            $unwind: {path: '$' + item.path, 'preserveNullAndEmptyArrays': true}
        });
    });
    args.push({
        $match:
            {
                Remark:{$ne: "首次检验"}
            }
    });





    addCondition(param,Org);
    return new Promise((resolve, reject) => {
        const entity = entityCollection.getEntity('QCTask');
        entity.Entity.aggregate(args, function (err, data) {
            if (err) {
                const newErr = new Error();
                newErr.level = 9;
                newErr.title = '查询库存记录错误';
                newErr.message = '查询失败，请检查查询参数！' + err.message;
                resolve([]);
            } else {
                resolve(data);
            }
        });
    });

    function addCondition(param,Org) {
        if (param.ItemMasterName) {
            const regExp = new RegExp(param.ItemMasterName);
            args.push({
                $match: {
                    'ItemMaster.Name': regExp
                }
            });
        }
        if (param.ItemMasterCode) {
            const regExp = new RegExp(param.ItemMasterCode);
            args.push({
                $match: {
                    'ItemMaster.Code': regExp
                }
            });
        }
        if (param.ItemMasterSpecification) {
            const regExp = new RegExp(param.ItemMasterSpecification);
            args.push({
                $match: {
                    'ItemMaster.Specification': regExp
                }
            });
        }
        if (param.Supplier) {
            const regExp = new RegExp(param.Supplier);
            args.push({
                $match: {
                    'Supplier.Code': regExp
                }
            });
        }
        if (/\d/.test(param._Status)) {
            args.push({
                $match: {
                    'Status': param._Status
                }
            });
        }
        if (param.LotNo) {
            const regExp = new RegExp(param.LotNo);
            args.push({
                $match: {
                    'LotNo': regExp
                }
            });
        }
        if (param.Checker) {
            const regExp = new RegExp(param.Checker);
            args.push({
                $match: {
                    'InspectedBy.Name': regExp
                }
            });
        }
        if (param.CompleteStartDate || param.CompleteEndDate) {
            let condition = {};
            if (param.CompleteStartDate) {
                condition['$gte'] = new Date(Date.parse(param.CompleteStartDate));
            }
            if (param.CompleteEndDate) {
                condition['$lte'] = new Date(Date.parse(param.CompleteEndDate));
            }
            args.unshift({
                $match: {
                    'CompleteTime': condition
                }
            });
        }
        if (/\d/.test(param.Result)) {
            args.push({
                $match: {
                    'QCConclusion': param.Result
                }
            });
        }
        if (param.Adjudicator) {
            const regExp = new RegExp(param.Adjudicator);
            args.push({
                $match: {
                    'JudgedBy.Name': regExp
                }
            });
        }

        if (param.JudgedOnStart || param.JudgedOnEnd) {
            let condition = {};
            if (param.JudgedOnStart) {
                condition['$gte'] = new Date(Date.parse(param.JudgedOnStart));
            }
            if (param.JudgedOnEnd) {
                condition['$lte'] = new Date(Date.parse(param.JudgedOnEnd));
            }
            args.unshift({
                $match: {
                    'JudgedOn': condition
                }
            });
        }

        if (param.Remark) {
            const regExp = new RegExp(param.Remark);
            args.push({
                $match: {
                    'Remark': regExp
                }
            });
        }

        if(Org){
            const regExp = new RegExp(Org);
            args.push({
                $match: {
                    'Org.Code': regExp
                }
            });
        }

    }
}
exports.updateQcTask = async function (req, res) {
    let starttimestr = req.body.startTime;
    let endtimestr = req.body.endTime;
    try {
        let starttime = new Date(starttimestr);
        let endtime = new Date(endtimestr);
        let qcTasks = await entityFun.find('QCTask', {"CreatedOn":  { $gt : starttime ,$lt:endtime }})
        for (let qcTask of qcTasks){
            const qcRecord = await  entityFun.findOne('QCRecord', {QCTask: qcTask._id })
            if (qcRecord){
                const rcvPlanRecord = await entityFun.findOne('RcvPlanRecord', {_id: qcTask.EntityID })
                if (rcvPlanRecord){
                    const rcvPlanLine  = await entityFun.findOne('RcvPlanLine', {RcvPlan: rcvPlanRecord.RcvPlan })
                    const buyer = rcvPlanLine.PrivateDescSeg10;
                    let str = rcvPlanLine.PrivateDescSeg6
                    const project = str.substring(0, str.indexOf('-'));
                    const assembly = rcvPlanLine.PrivateDescSeg1;
                    const assembly_detail = rcvPlanLine.PrivateDescSeg2;
                    const latest_version = rcvPlanLine.PrivateDescSeg4;
                    qcTask.PrivateDescSeg15 = qcRecord.PrivateDescSeg9;
                    qcTask.PrivateDescSeg16 = buyer;
                    qcTask.PrivateDescSeg17 = project;
                    qcTask.PrivateDescSeg18 = assembly;
                    qcTask.PrivateDescSeg19 = assembly_detail;
                    qcTask.PrivateDescSeg20 = latest_version;
                }
            }
            qcTask.RowStatus = 3;
        }
        await entityFun.batchSaveByTran([
            {
                EntityName: 'QCTask', Records: qcTasks
            }
        ]);
    res.json({
        Data: qcTasks.size,
        Error: null
    }
    );
} catch (error) {
    res.json({
        Data: null,
        Error: error
    });
}
}