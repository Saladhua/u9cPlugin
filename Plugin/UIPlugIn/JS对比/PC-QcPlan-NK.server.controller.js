const entityFun = require('../../../../Base/server/controllers/EntityFunction');
const entityCollection = require('../../../../Base/server/controllers/EntityCollection');
exports.reportQuery = async function (req, res) {
    let {body: param, Context} = req;
    try {
        const qcTasks = await getAggregateQcTask(req.body,Context.Org.Code);
        const retQcTasks = [];
        let curr = new Date().getTime();
        const RcvPlanRecordLine =await getRcvPlanRecordLine(req.body,Context.Org.Code);
        let lineDate = [];
        if (Array.isArray(qcTasks) && qcTasks.length) {
            for (let item of qcTasks) {
                //item.RcvPlanRecord = item.RcvPlanRecord[item.RcvPlanRecord.length - 1]; 
                //入库人 2024/1/4更新
                let RcvName = '';
                let RcvTime = null;
                let isOut = '';
                let ot = req.body._isOut == 1 ? '是':'否';

                let WlD10 = '';

                let WlD11 = '';

                const qcRecord = await  entityFun.findOne('QCRecord', {QCTask: item._id }) 

                if(item.RcvPlanRecord && Context.Org.Code == '10'){ 
                    for(let line of RcvPlanRecordLine){
                        if(item.RcvPlanRecord._id.id.toString() == line.RcvPlanRecord.id.toString() && line.ItemMaster.toString() == item.ItemMaster._id.toString()){
                            RcvName = line.CreatedByName;
                            RcvTime = line.CreatedOn;
                            item.pro = line.PrivateDescSeg6;
                        }
                    } 
                } 
                //item.RcvInTime = new Date(RcvTime.getTime() + 8 * 60 * 60 * 1000);

                if(RcvTime && Context.Org.Code == '10'){
                    item.RcvInTime = `${RcvTime.getFullYear()}.${padZero(RcvTime.getMonth() + 1)}.${padZero(RcvTime.getDate())} ${padZero(RcvTime.getHours())}:${padZero(RcvTime.getMinutes())}:${padZero(RcvTime.getSeconds())}`
                    if(item.JudgedOn){
                        RcvTime = ((RcvTime.getTime() - item.JudgedOn.getTime()) / 3600000).toFixed(2) ;
                    }else {
                        RcvTime = ((RcvTime.getTime() - item.CompleteTime.getTime()) / 3600000).toFixed(2) ;
                    }
                    isOut = RcvTime > 48 ? '是' : '否'
                }
                //------------------------ 
                if (qcRecord){
                    item.WlD10 = qcRecord.PrivateDescSeg10;
                    item.WlD11 = qcRecord.PrivateDescSeg11;
                }

                item.isOut = isOut;
                item.RcvTime = RcvTime;
                item.RcvName = RcvName;
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
                /*
                item.Durationtime =
                    (day ? day + '天' : '') +
                    (hour ? hour + '小时' : '') +
                    (minute ? minute + '分钟' : '') +
                    (second ? second + '秒' : '') ;
                 */
                item.Durationtime = (timestamp / 3600000).toFixed(2)
                //if(!((req.body.Project && item.project.includes(req.body.Project)) || !req.body.Project))continue;
                if(Context.Org.Code == item.Org.Code && (req.body._isOut == null || req.body._isOut == '' || ot == isOut)){
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


function padZero(num) {
    return num < 10 ? '0' + num : num;
}

function getRcvPlanRecordLine(param,Org){
    let args = [];
    addCondition(param);
    return new Promise((resolve, reject) => {
        const entity = entityCollection.getEntity('RcvPlanRecordLine');
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


    function addCondition(param) {

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
                    'CreatedOn': condition
                }
            });
        }
    }
}

function getAggregateQcTask(param,Org) {
    let args = [];
    const populate = [
        {path: 'ItemMaster', select: 'Code Name Specification Org PrivateDescSeg1'},
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
        //{path: 'QcRecord', select: 'PrivateDescSeg10 PrivateDescSeg8'},
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
    //2024-1-04新增
    args.push({
        $lookup:
            {
                from: 'RcvPlanRecord',
                let: {
                    'lid': '$LotNo'
                },
                pipeline: [
                    {
                        $match: {
                            $expr: {$eq: ['$LotNo', '$$lid']}

                        }
                    },

                    {$project:{'_id':1,'Status':1}}
                ],
                as: 'RcvPlanRecord'
            }

    });
    args.push({
        $unwind: {path: '$RcvPlanRecord', 'preserveNullAndEmptyArrays': true}
    });

    /*
    args.push({
        $lookup:
            {
                from: 'RcvPlanRecordLine',
                let: {
                    'lid': '$RcvPlanRecord',
                    'litem':'$ItemMaster'
                },
                pipeline: [
                    {
                        $match: {
                            $expr: {
                                $and: [
                                    { $eq: ['$RcvPlanRecord._id', '$$lid'] },
                                    { $eq: ['$ItemMaster._id', '$$litem'] }
                                ]
                            }

                        }
                    },

                    {$project:{'_id':1,'Status':1}}
                ],
                as: 'RcvPlanRecordLine'
            }

    });
    args.push({
        $unwind: {path: '$RcvPlanRecordLine', 'preserveNullAndEmptyArrays': true}
    });

     */
    /*
    args.push({
        $lookup:
            {
                from: 'RCVPlan',
                let: {
                    'lid': '$RcvPlanRecord.RCVPlan'
                },
                pipeline: [
                    {
                        $match: {
                            $expr: {$eq: ['$_id', '$$lid']}

                        }
                    },

                    {$project:{'_id':1}}
                ],
                as: 'RCVPlan'
            }

    });

     */
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
        if (/\d/.test(param.RcvPlanRecordStatus)) {
            //const regExp = new RegExp(param.RcvPlanRecordStatus);
            args.push({
                $match: {
                    'RcvPlanRecord.Status': param.RcvPlanRecordStatus
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

        if (param.Project) {
            const regExp = new RegExp(param.Project);
            args.push({
                $match: {
                    'PrivateDescSeg17': regExp
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